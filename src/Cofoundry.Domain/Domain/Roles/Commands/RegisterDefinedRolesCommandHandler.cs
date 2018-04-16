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

namespace Cofoundry.Domain
{
    /// <summary>
    /// Registers new roles defined in code via IRoleDefinition and initializes
    /// permissions when an IRoleInitializer has been implemented.
    /// </summary>
    public class RegisterDefinedRolesCommandHandler
        : IAsyncCommandHandler<RegisterDefinedRolesCommand>
        , IPermissionRestrictedCommandHandler<RegisterDefinedRolesCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IRoleCache _roleCache;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IEnumerable<IRoleDefinition> _roleDefinitions;
        private readonly IRoleInitializerFactory _roleInitializerFactory;
        private readonly IPermissionRepository _permissionRepository;

        public RegisterDefinedRolesCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            IRoleCache roleCache,
            IPermissionValidationService permissionValidationService,
            IEnumerable<IRoleDefinition> roleDefinitions,
            IRoleInitializerFactory roleInitializerFactory,
            IPermissionRepository permissionRepository
            )
        {
            _dbContext = dbContext;
            _commandExecutor = commandExecutor;
            _roleCache = roleCache;
            _permissionValidationService = permissionValidationService;
            _roleDefinitions = roleDefinitions;
            _roleInitializerFactory = roleInitializerFactory;
            _permissionRepository = permissionRepository;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(RegisterDefinedRolesCommand command, IExecutionContext executionContext)
        {
            DetectDuplicateRoles();

            var existingRoles = await _dbContext
                .Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(p => p.Permission)
                .ToListAsync();

            var rolesWithSpecialistCodes = existingRoles
                .Where(r => !string.IsNullOrEmpty(r.RoleCode))
                .ToDictionary(r => r.RoleCode.ToUpperInvariant());

            var requiresUpdate = command.UpdateExistingRoles || _roleDefinitions.Any(d => !rolesWithSpecialistCodes.ContainsKey(d.RoleCode.ToUpperInvariant()));
            if (!requiresUpdate) return;

            var allDbPermissions = await _dbContext
                .Permissions
                .ToListAsync();

            await EnsureUserAreaExistsAndValidatePermissionAsync(existingRoles, executionContext);
            var allPermissions = _permissionRepository.GetAll();
            
            foreach (var roleDefinition in _roleDefinitions)
            {
                var dbRole = rolesWithSpecialistCodes.GetOrDefault(roleDefinition.RoleCode.ToUpperInvariant());

                if (dbRole == null)
                {
                    ValidateRole(existingRoles, roleDefinition);
                    dbRole = MapAndAddRole(roleDefinition);
                    await UpdatePermissionsAsync(dbRole, roleDefinition, command, allPermissions, allDbPermissions, executionContext);
                }
                else if (command.UpdateExistingRoles)
                {
                    await UpdatePermissionsAsync(dbRole, roleDefinition, command, allPermissions, allDbPermissions, executionContext);
                }
            }

            await _dbContext.SaveChangesAsync();
            _roleCache.Clear();
        }

        private async Task UpdatePermissionsAsync(
            Role dbRole, 
            IRoleDefinition roleDefinition,
            RegisterDefinedRolesCommand command,
            IEnumerable<IPermission> allPermissions,
            List<Permission> allDbPermissions,
            IExecutionContext executionContext
            )
        {
            var roleInitializer = _roleInitializerFactory.Create(roleDefinition);
            if (roleInitializer == null) return;

            var permissionsToInclude = roleInitializer
                .GetPermissions(allPermissions)
                .ToList();

            // Remove permissions
            if (command.UpdateExistingRoles)
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
            ValidatePermissions(permissionsToInclude);

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
                var dbPermission = allDbPermissions
                    .Where(p => uniquePermissionCode == p.GetUniqueCode())
                    .SingleOrDefault();

                // Create if not exists
                if (dbPermission == null)
                {
                    dbPermission = new Permission();
                    dbPermission.PermissionCode = permissionToAdd.PermissionType.Code;

                    if (permissionToAdd is IEntityPermission)
                    {
                        var definitionCode = ((IEntityPermission)permissionToAdd).EntityDefinition.EntityDefinitionCode;
                        await _commandExecutor.ExecuteAsync(new EnsureEntityDefinitionExistsCommand(definitionCode), executionContext);
                        dbPermission.EntityDefinitionCode = definitionCode;
                    }

                    allDbPermissions.Add(dbPermission);
                    _dbContext.Permissions.Add(dbPermission);
                }

                var rolePermission = new RolePermission();
                rolePermission.Permission = dbPermission;
                dbRole.RolePermissions.Add(rolePermission);
            }
        }

        /// <summary>
        /// Validation ensures that we don't have any entity permissions
        /// that have elevated access without first beiung granted a read 
        /// permission. E.g. having 'UpdatePage' permission without also
        /// having 'ReadPage' permission.
        /// </summary>
        private void ValidatePermissions(IEnumerable<IPermission> permissions)
        {
            var entityWithoutReadPermission = permissions
                .FilterEntityPermissions()
                .Where(p => !string.IsNullOrWhiteSpace(p.EntityDefinition?.EntityDefinitionCode))
                .GroupBy(p => p.EntityDefinition.EntityDefinitionCode)
                .Where(g => !g.Any(p => p.PermissionType?.Code == CommonPermissionTypes.ReadPermissionCode));

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
                throw new PropertyValidationException("Role title cannot be empty", nameof(IRoleDefinition.Title));
            }

            if (string.IsNullOrWhiteSpace(roleDefinition.RoleCode))
            {
                throw new PropertyValidationException("Role RoleCode cannot be empty", nameof(IRoleDefinition.RoleCode));
            }

            if (roleDefinition.RoleCode.Length != 3)
            {
                throw new PropertyValidationException("Role RoleCode must be 3 characters in length", nameof(IRoleDefinition.RoleCode));
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
                if (!existingRoles
                        .Any(r => r.UserAreaCode == userAreaCode))
                {
                    await _commandExecutor.ExecuteAsync(new EnsureUserAreaExistsCommand(userAreaCode), executionContext);
                }
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(RegisterDefinedRolesCommand command)
        {
            yield return new RoleCreatePermission();
        }

        #endregion
    }
}
