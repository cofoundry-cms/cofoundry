using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class UpdateRoleCommandHandler 
        : IAsyncCommandHandler<UpdateRoleCommand>
        , IPermissionRestrictedCommandHandler<UpdateRoleCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRoleCache _roleCache;
        private readonly ICommandExecutor _commandExecutor;

        public UpdateRoleCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPermissionRepository permissionRepository,
            IRoleCache roleCache,
            ICommandExecutor commandExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _permissionRepository = permissionRepository;
            _roleCache = roleCache;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdateRoleCommand command, IExecutionContext executionContext)
        {
            ValidatePermissions(command);

            var role = await QueryRole(command).SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(role, command.RoleId);

            var isUnique = await _queryExecutor.ExecuteAsync(GetUniqueQuery(command, role));
            ValidateIsUnique(isUnique);

            MapRole(command, role);
            await MergePermissions(command, role);
            await _dbContext.SaveChangesAsync();

            _roleCache.Clear(role.RoleId);
        }

        #endregion

        #region helpers

        private void MapRole(UpdateRoleCommand command, Role role)
        {
            role.Title = command.Title.Trim();
        }

        private void ValidateIsUnique(bool isUnique)
        {
            if (!isUnique)
            {
                throw new PropertyValidationException("A role with this title already exists", "Title");
            }
        }

        private IsRoleTitleUniqueQuery GetUniqueQuery(UpdateRoleCommand command, Role role)
        {
            return new IsRoleTitleUniqueQuery()
            {
                RoleId = command.RoleId,
                UserAreaCode = role.UserAreaCode,
                Title = command.Title.Trim()
            };
        }

        private IQueryable<Role> QueryRole(UpdateRoleCommand command)
        {
            return _dbContext
                .Roles
                .Include(r => r.Permissions)
                .FilterById(command.RoleId);
        }

        private async Task MergePermissions(UpdateRoleCommand command, Role role)
        {
            // Deletions
            var permissionsToRemove = role
                .Permissions
                .Where(p => !command.Permissions.Any(cp => 
                            cp.PermissionCode == p.PermissionCode 
                            && (string.IsNullOrWhiteSpace(p.EntityDefinitionCode) || cp.EntityDefinitionCode == p.EntityDefinitionCode)
                            ))
                .ToList();

            foreach (var permissionToRemove in permissionsToRemove)
            {
                role.Permissions.Remove(permissionToRemove);
            }

            // Additions
            var permissionsToAdd = command
                .Permissions
                .Where(p => !role.Permissions.Any(cp => 
                            cp.PermissionCode == p.PermissionCode 
                            && (string.IsNullOrWhiteSpace(p.EntityDefinitionCode) || cp.EntityDefinitionCode == p.EntityDefinitionCode)));
            
            if (permissionsToAdd.Any())
            {
                // create a unique token to use for lookup
                var permissionToAddTokens = permissionsToAdd
                    .Select(p => p.EntityDefinitionCode + p.PermissionCode)
                    .ToList();

                // Get permissions from the db
                var dbPermissions = await _dbContext
                    .Permissions
                    .Where(p => permissionToAddTokens.Contains(p.EntityDefinitionCode + p.PermissionCode))
                    .ToListAsync();

                foreach (var permissionToAdd in permissionsToAdd)
                {
                    // Get Db permission
                    var dbPermission = dbPermissions
                            .Where(p => permissionToAdd.PermissionCode == p.PermissionCode
                                    && (string.IsNullOrEmpty(permissionToAdd.EntityDefinitionCode) || permissionToAdd.EntityDefinitionCode == p.EntityDefinitionCode)
                                )
                            .SingleOrDefault();

                    // Create if not exists
                    if (dbPermission == null)
                    {
                        var codePermission = _permissionRepository.GetByCode(permissionToAdd.PermissionCode, permissionToAdd.EntityDefinitionCode);
                        dbPermission = new Permission();
                        dbPermission.PermissionCode = codePermission.PermissionType.Code;

                        if (codePermission is IEntityPermission)
                        {
                            var definitionCode = ((IEntityPermission)codePermission).EntityDefinition.EntityDefinitionCode;
                            await _commandExecutor.ExecuteAsync(new EnsureEntityDefinitionExistsCommand(definitionCode));
                            dbPermission.EntityDefinitionCode = definitionCode;
                        }
                    }

                    role.Permissions.Add(dbPermission);
                }
            }
        }

        private void ValidatePermissions(UpdateRoleCommand command)
        {
            if (!EnumerableHelper.IsNullOrEmpty(command.Permissions))
            {
                var entityWithoutReadPermission = command.Permissions
                    .Where(p => !string.IsNullOrEmpty(p.EntityDefinitionCode))
                    .GroupBy(p => p.EntityDefinitionCode)
                    .Where(g => !g.Any(p => p.PermissionCode == CommonPermissionTypes.ReadPermissionCode));

                foreach (var entity in entityWithoutReadPermission)
                {
                    var entityCode = entity.First().EntityDefinitionCode;
                    var readPermission = _permissionRepository.GetByEntityAndPermissionType(entityCode, CommonPermissionTypes.ReadPermissionCode);

                    if (readPermission != null)
                    {
                        var msg = "Read permissions must be granted to entity " + entityCode + " in order to assign additional permissions";
                        throw new ValidationException(msg);
                    }
                }
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateRoleCommand command)
        {
            yield return new RoleUpdatePermission();
        }

        #endregion
    }
}
