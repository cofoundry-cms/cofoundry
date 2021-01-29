using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Registers new roles and permissions defined in code and initializes
    /// permissions when an IRoleInitializer has been implemented.
    /// </summary>
    public class RegisterPermissionsAndRolesCommandHandler
        : ICommandHandler<RegisterPermissionsAndRolesCommand>
        , IPermissionRestrictedCommandHandler<RegisterPermissionsAndRolesCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IRoleCache _roleCache;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IEnumerable<IRoleDefinition> _roleDefinitions;
        private readonly IRoleInitializerFactory _roleInitializerFactory;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public RegisterPermissionsAndRolesCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            IRoleCache roleCache,
            IPermissionValidationService permissionValidationService,
            IEnumerable<IRoleDefinition> roleDefinitions,
            IRoleInitializerFactory roleInitializerFactory,
            IPermissionRepository permissionRepository,
            IEntityDefinitionRepository entityDefinitionRepository,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _commandExecutor = commandExecutor;
            _roleCache = roleCache;
            _permissionValidationService = permissionValidationService;
            _roleDefinitions = roleDefinitions;
            _roleInitializerFactory = roleInitializerFactory;
            _permissionRepository = permissionRepository;
            _entityDefinitionRepository = entityDefinitionRepository;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(RegisterPermissionsAndRolesCommand command, IExecutionContext executionContext)
        {
            DetectDuplicateRoles();

            // ENTITY DEFINITIONS

            var dbEntityDefinitions = await _dbContext
                .EntityDefinitions
                .ToDictionaryAsync(e => e.EntityDefinitionCode);

            await EnsureAllEntityDefinitionsExistAsync(dbEntityDefinitions);

            // PERMISSIONS

            // permissions already registered in the database
            var dbPermissions = await _dbContext
                .Permissions
                .ToDictionaryAsync(p => p.GetUniqueCode());

            // code-based permission objects
            var codePermissions = _permissionRepository.GetAll();

            var newCodePermissions = codePermissions
                .Where(p => !dbPermissions.ContainsKey(p.GetUniqueCode()))
                .ToList();

            // Add new permissions to db

            AddNewPermissionsToDb(dbEntityDefinitions, dbPermissions, newCodePermissions);

            // ROLES

            var dbRoles = await _dbContext
                .Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(p => p.Permission)
                .ToListAsync();

            var dbRolesWithCodes = dbRoles
                .Where(r => !string.IsNullOrEmpty(r.RoleCode))
                .ToDictionary(r => r.RoleCode.ToUpperInvariant());

            await EnsureUserAreaExistsAndValidatePermissionAsync(dbRoles, executionContext);

            foreach (var roleDefinition in _roleDefinitions)
            {
                var dbRole = dbRolesWithCodes.GetOrDefault(roleDefinition.RoleCode.ToUpperInvariant());

                if (dbRole == null)
                {
                    // New role
                    ValidateRole(dbRoles, roleDefinition);
                    dbRole = MapAndAddRole(roleDefinition);
                    UpdatePermissions(dbRole, roleDefinition, codePermissions, dbPermissions, dbEntityDefinitions, false);
                }
                else if (command.UpdateExistingRoles)
                {
                    // Existing role, to be updated to match initializer exactly
                    UpdatePermissions(dbRole, roleDefinition, codePermissions, dbPermissions, dbEntityDefinitions, true);
                }
                else
                {
                    // Update for new permissions only
                    UpdatePermissions(dbRole, roleDefinition, newCodePermissions, dbPermissions, dbEntityDefinitions, false);
                }
            }

            await _dbContext.SaveChangesAsync();
            _transactionScopeFactory.QueueCompletionTask(_dbContext, _roleCache.Clear);
        }

        private void AddNewPermissionsToDb(
            Dictionary<string, EntityDefinition> dbEntityDefinitions, 
            Dictionary<string, Permission> dbPermissions, 
            List<IPermission> newCodePermissions
            )
        {
            foreach (var permissionToAdd in newCodePermissions)
            {
                var uniquePermissionCode = permissionToAdd.GetUniqueCode();
                // Create if not exists
                var dbPermission = new Permission();
                dbPermission.PermissionCode = permissionToAdd.PermissionType.Code;

                if (permissionToAdd is IEntityPermission entityPermissionToAdd)
                {
                    dbPermission.EntityDefinition = dbEntityDefinitions.GetOrDefault(entityPermissionToAdd.EntityDefinition.EntityDefinitionCode);

                    if (dbPermission.EntityDefinition == null)
                    {
                        throw new Exception($"Cannot add permission. Entity definition with the code {entityPermissionToAdd.EntityDefinition.EntityDefinitionCode} was expected but could not be found.");
                    }
                }

                dbPermissions.Add(uniquePermissionCode, dbPermission);
                _dbContext.Permissions.Add(dbPermission);
            }
        }

        /// <summary>
        /// Entity definitions db records are created on the fly so we need to make sure
        /// any new ones exist before we add permissions to them.
        /// 
        /// Typically we'd use EnsureEntityDefinitionExistsCommand to create the entity
        /// definition, but since this command also creates permissions we need to do this
        /// manually.
        /// </summary>
        private async Task EnsureAllEntityDefinitionsExistAsync(
            Dictionary<string, EntityDefinition> dbDefinitions
            )
        {
            var codeDefinitions = _entityDefinitionRepository.GetAll();

            var newEntityCodes = codeDefinitions
                .Select(d => d.EntityDefinitionCode)
                .Where(d => !dbDefinitions.ContainsKey(d));

            if (!newEntityCodes.Any()) return;

            foreach (var definitionCode in newEntityCodes)
            {
                // get the entity definition class
                var entityDefinition = _entityDefinitionRepository.GetByCode(definitionCode);

                // create a matching db record
                var dbDefinition = new EntityDefinition()
                {
                    EntityDefinitionCode = entityDefinition.EntityDefinitionCode,
                    Name = entityDefinition.Name
                };

                _dbContext.EntityDefinitions.Add(dbDefinition);
                dbDefinitions.Add(dbDefinition.EntityDefinitionCode, dbDefinition);
            }

            await _dbContext.SaveChangesAsync();
        }

        private void UpdatePermissions(
            Role dbRole, 
            IRoleDefinition roleDefinition,
            IEnumerable<IPermission> codePermissions,
            Dictionary<string, Permission> dbPermissions,
            Dictionary<string, EntityDefinition> dbEntityDefinitions,
            bool allowDeletions
            )
        {
            var roleInitializer = _roleInitializerFactory.Create(roleDefinition);
            if (roleInitializer == null) return;

            var permissionsToInclude = roleInitializer
                .GetPermissions(codePermissions)
                .ToList();

            // Remove permissions
            if (allowDeletions)
            {
                var permissionsToRemove = dbRole
                    .RolePermissions
                    .Where(p => !permissionsToInclude.Any(i => i.GetUniqueCode() == p.Permission.GetUniqueCode()))
                    .ToList();

                foreach (var permissonToRemove in permissionsToRemove)
                {
                    dbRole.RolePermissions.Remove(permissonToRemove);
                }
            }

            if (!permissionsToInclude.Any()) return;
            ValidatePermissions(dbRole.RolePermissions, permissionsToInclude);

            // add new permissions
            IEnumerable<IPermission> permissionsToAdd;

            if (dbRole.RolePermissions.Count == 0)
            {
                permissionsToAdd = permissionsToInclude;
            }
            else
            {
                permissionsToAdd = permissionsToInclude
                    .Where(i => !dbRole.RolePermissions.Any(p => p.Permission.GetUniqueCode() == i.GetUniqueCode()));
            }

            foreach (var permissionToAdd in permissionsToAdd)
            {
                var uniquePermissionCode = permissionToAdd.GetUniqueCode();
                var dbPermission = dbPermissions.GetOrDefault(uniquePermissionCode);

                if (dbPermission == null)
                {
                    throw new Exception("dbPermissions lookup does not contain the specified permission, but was expected: " + uniquePermissionCode);
                }

                var rolePermission = new RolePermission();
                rolePermission.Permission = dbPermission;
                dbRole.RolePermissions.Add(rolePermission);
            }
        }

        /// <summary>
        /// Validation ensures that we don't have any entity permissions
        /// that have elevated access without first being granted a read 
        /// permission. E.g. having 'UpdatePage' permission without also
        /// having 'ReadPage' permission.
        /// </summary>
        private void ValidatePermissions(ICollection<RolePermission> existingPermissions, IEnumerable<IPermission> permissions)
        {
            var entityWithoutReadPermission = permissions
                .FilterEntityPermissions()
                .Where(p => !string.IsNullOrWhiteSpace(p.EntityDefinition?.EntityDefinitionCode))
                .GroupBy(p => p.EntityDefinition.EntityDefinitionCode)
                .Where(g => 
                    !g.Any(p => p.PermissionType?.Code == CommonPermissionTypes.ReadPermissionCode) 
                    && !existingPermissions.Any(p => p.Permission.EntityDefinitionCode == g.Key && p.Permission.PermissionCode == CommonPermissionTypes.ReadPermissionCode));

            foreach (var entity in entityWithoutReadPermission)
            {
                var entityCode = entity.First().EntityDefinition.EntityDefinitionCode;
                var readPermission = _permissionRepository.GetByEntityAndPermissionType(entityCode, CommonPermissionTypes.ReadPermissionCode);

                if (readPermission != null)
                {
                    var msg = "Read permissions must be granted to entity " + entityCode + " in order to assign additional permissions";
                    throw new ValidationException(msg);
                }
            }
        }

        private void DetectDuplicateRoles()
        {
            var duplicateDefinition = _roleDefinitions
                    .GroupBy(d => d.RoleCode)
                    .Where(d => d.Count() > 1)
                    .FirstOrDefault();

            if (duplicateDefinition != null)
            {
                var message = $"Duplicate role definitions encountered. { duplicateDefinition.Count() } roles defined with the code '{ duplicateDefinition.First().RoleCode}'";
                throw new InvalidRoleDefinitionException(message, duplicateDefinition.FirstOrDefault(), _roleDefinitions);
            }
        }

        private Role MapAndAddRole(IRoleDefinition roleDefinition)
        {
            var dbRole = new Role();
            dbRole.Title = roleDefinition.Title.Trim();
            dbRole.UserAreaCode = roleDefinition.UserAreaCode;
            dbRole.RoleCode = roleDefinition.RoleCode;

            _dbContext.Roles.Add(dbRole);
            return dbRole;
        }

        private static void ValidateRole(List<Role> existingRoles, IRoleDefinition roleDefinition)
        {
            if (string.IsNullOrWhiteSpace(roleDefinition.Title))
            {
                throw ValidationErrorException.CreateWithProperties("Role title cannot be empty", nameof(IRoleDefinition.Title));
            }

            if (string.IsNullOrWhiteSpace(roleDefinition.RoleCode))
            {
                throw ValidationErrorException.CreateWithProperties("Role RoleCode cannot be empty", nameof(IRoleDefinition.RoleCode));
            }

            if (roleDefinition.RoleCode.Length != 3)
            {
                throw ValidationErrorException.CreateWithProperties("Role RoleCode must be 3 characters in length", nameof(IRoleDefinition.RoleCode));
            }
            if (existingRoles
                    .Any(r =>
                        r.Title.Equals(roleDefinition.Title?.Trim(), StringComparison.OrdinalIgnoreCase)
                        && r.UserAreaCode == roleDefinition.UserAreaCode)
                        )
            {
                throw new UniqueConstraintViolationException($"A role with the title '{ roleDefinition.Title }' already exists", nameof(Role.Title));
            }
        }

        private async Task EnsureUserAreaExistsAndValidatePermissionAsync(
            List<Role> existingRoles, 
            IExecutionContext executionContext
            )
        {
            var allUserAreaCodes = _roleDefinitions
                .Select(a => a.UserAreaCode)
                .Distinct();

            foreach (var userAreaCode in allUserAreaCodes)
            {
                // Make sure we have permissions to this user area before we start adding roles
                _permissionValidationService.EnforceHasPermissionToUserArea(userAreaCode, executionContext.UserContext);

                // If the user area already exists on a role then we don't need to check it
                if (!existingRoles.Any(r => r.UserAreaCode == userAreaCode))
                {
                    await _commandExecutor.ExecuteAsync(new EnsureUserAreaExistsCommand(userAreaCode), executionContext);
                }
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(RegisterPermissionsAndRolesCommand command)
        {
            yield return new RoleCreatePermission();
        }

        #endregion
    }
}
