using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;
using Cofoundry.Core;
using Cofoundry.Core.Data;

namespace Cofoundry.Domain.Internal
{
    public class UpdateRoleCommandHandler 
        : ICommandHandler<UpdateRoleCommand>
        , IPermissionRestrictedCommandHandler<UpdateRoleCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRoleCache _roleCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdateRoleCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPermissionRepository permissionRepository,
            IRoleCache roleCache,
            ICommandExecutor commandExecutor,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _permissionRepository = permissionRepository;
            _roleCache = roleCache;
            _commandExecutor = commandExecutor;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdateRoleCommand command, IExecutionContext executionContext)
        {
            ValidatePermissions(command);

            var role = await QueryRole(command).SingleOrDefaultAsync();
            EntityNotFoundException.ThrowIfNull(role, command.RoleId);

            var isUnique = await _queryExecutor.ExecuteAsync(GetUniqueQuery(command, role), executionContext);
            ValidateIsUnique(isUnique);

            MapRole(command, role);
            await MergePermissionsAsync(command, role, executionContext);
            await _dbContext.SaveChangesAsync();

            _transactionScopeFactory.QueueCompletionTask(_dbContext, () => _roleCache.Clear(command.RoleId));
        }

        private void MapRole(UpdateRoleCommand command, Role role)
        {
            role.Title = command.Title.Trim();
        }

        private void ValidateIsUnique(bool isUnique)
        {
            if (!isUnique)
            {
                throw ValidationErrorException.CreateWithProperties("A role with this title already exists", "Title");
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
                .Include(r => r.RolePermissions)
                .ThenInclude(p => p.Permission)
                .FilterById(command.RoleId);
        }

        private async Task MergePermissionsAsync(UpdateRoleCommand command, Role role, IExecutionContext executionContext)
        {
            // Deletions
            var permissionsToRemove = role
                .RolePermissions
                .Where(p => !command.Permissions.Any(cp => 
                            cp.PermissionCode == p.Permission.PermissionCode 
                            && (string.IsNullOrWhiteSpace(p.Permission.EntityDefinitionCode) || cp.EntityDefinitionCode == p.Permission.EntityDefinitionCode)
                            ))
                .ToList();

            foreach (var permissionToRemove in permissionsToRemove)
            {
                _dbContext.RolePermissions.Remove(permissionToRemove);
            }

            // Additions
            var permissionsToAdd = command
                .Permissions
                .Where(p => !role.RolePermissions.Any(cp => 
                            cp.Permission.PermissionCode == p.PermissionCode 
                            && (string.IsNullOrWhiteSpace(p.EntityDefinitionCode) || cp.Permission.EntityDefinitionCode == p.EntityDefinitionCode)));
            
            if (permissionsToAdd.Any())
            {
                // create a unique token to use for lookup
                var permissionToAddTokens = permissionsToAdd
                    .Select(p => p.EntityDefinitionCode + p.PermissionCode)
                    .ToList();

                // Get permissions from the db
                var dbPermissions = await _dbContext
                    .Permissions
                    .Where(p => 
                        permissionToAddTokens.Contains((p.EntityDefinitionCode ?? "") + p.PermissionCode))
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
                            await _commandExecutor.ExecuteAsync(new EnsureEntityDefinitionExistsCommand(definitionCode), executionContext);
                            dbPermission.EntityDefinitionCode = definitionCode;
                        }
                    }

                    var rolePermission = new RolePermission();
                    rolePermission.Permission = dbPermission;
                    role.RolePermissions.Add(rolePermission);
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
