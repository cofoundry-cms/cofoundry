using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Registers new roles and permissions defined in code and initializes
/// permissions when an IRoleInitializer has been implemented.
/// </summary>
public class RegisterPermissionsAndRolesCommandHandler
    : ICommandHandler<RegisterPermissionsAndRolesCommand>
    , IPermissionRestrictedCommandHandler<RegisterPermissionsAndRolesCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICommandExecutor _commandExecutor;
    private readonly IRoleCache _roleCache;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly IRoleDefinitionRepository _roleDefinitionRepository;
    private readonly IRolePermissionInitializerFactory _rolePermissionInitializerFactory;
    private readonly IPermissionSetBuilderFactory _permissionSetBuilderFactory;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;
    private readonly ITransactionScopeManager _transactionScopeManager;

    public RegisterPermissionsAndRolesCommandHandler(
        CofoundryDbContext dbContext,
        ICommandExecutor commandExecutor,
        IRoleCache roleCache,
        IPermissionValidationService permissionValidationService,
        IRoleDefinitionRepository roleDefinitionRepository,
        IRolePermissionInitializerFactory rolePermissionInitializerFactory,
        IPermissionSetBuilderFactory permissionSetBuilderFactory,
        IPermissionRepository permissionRepository,
        IEntityDefinitionRepository entityDefinitionRepository,
        ITransactionScopeManager transactionScopeManager
        )
    {
        _dbContext = dbContext;
        _commandExecutor = commandExecutor;
        _roleCache = roleCache;
        _permissionValidationService = permissionValidationService;
        _roleDefinitionRepository = roleDefinitionRepository;
        _rolePermissionInitializerFactory = rolePermissionInitializerFactory;
        _permissionSetBuilderFactory = permissionSetBuilderFactory;
        _permissionRepository = permissionRepository;
        _entityDefinitionRepository = entityDefinitionRepository;
        _transactionScopeManager = transactionScopeManager;
    }

    public async Task ExecuteAsync(RegisterPermissionsAndRolesCommand command, IExecutionContext executionContext)
    {
        // ENTITY DEFINITIONS

        var dbEntityDefinitions = await _dbContext
            .EntityDefinitions
            .ToDictionaryAsync(e => e.EntityDefinitionCode);

        await EnsureAllEntityDefinitionsExistAsync(dbEntityDefinitions);

        // PERMISSIONS

        // permissions already registered in the database
        var dbPermissions = await _dbContext
            .Permissions
            .ToDictionaryAsync(p => p.GetRequiredUniqueCode());

        // code-based permission objects
        var codePermissions = _permissionRepository.GetAll();

        var newCodePermissions = codePermissions
            .Where(p => !dbPermissions.ContainsKey(p.GetUniqueIdentifier()))
            .ToArray();

        // Add new permissions to db

        AddNewPermissionsToDb(dbEntityDefinitions, dbPermissions, newCodePermissions);

        // ROLES

        var dbRoles = await _dbContext
            .Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(p => p.Permission)
            .ToArrayAsync();

        var dbRolesWithCodes = dbRoles
            .Where(r => !string.IsNullOrEmpty(r.RoleCode))
            .ToDictionary(r => r.RoleCode!.ToUpperInvariant());

        await EnsureUserAreaExistsAndValidatePermissionAsync(dbRoles, executionContext);

        foreach (var roleDefinition in _roleDefinitionRepository.GetAll())
        {
            var dbRole = dbRolesWithCodes.GetValueOrDefault(roleDefinition.RoleCode.ToUpperInvariant());

            if (dbRole == null)
            {
                // New role
                ValidateRole(dbRoles, roleDefinition);
                dbRole = MapAndAddRole(roleDefinition);
                UpdatePermissions(dbRole, roleDefinition, codePermissions, dbPermissions, false);
            }
            else if (command.UpdateExistingRoles)
            {
                // Existing role, to be updated to match initializer exactly
                UpdatePermissions(dbRole, roleDefinition, codePermissions, dbPermissions, true);
            }
            else
            {
                // Update for new permissions only
                UpdatePermissions(dbRole, roleDefinition, newCodePermissions, dbPermissions, false);
            }
        }

        await _dbContext.SaveChangesAsync();
        _transactionScopeManager.QueueCompletionTask(_dbContext, _roleCache.Clear);
    }

    private void AddNewPermissionsToDb(
        IReadOnlyDictionary<string, EntityDefinition> dbEntityDefinitions,
        Dictionary<string, Permission> dbPermissions,
        IReadOnlyCollection<IPermission> newCodePermissions
        )
    {
        foreach (var permissionToAdd in newCodePermissions)
        {
            var uniquePermissionCode = permissionToAdd.GetUniqueIdentifier();
            // Create if not exists
            var dbPermission = new Permission();
            dbPermission.PermissionCode = permissionToAdd.PermissionType.Code;

            if (permissionToAdd is IEntityPermission entityPermissionToAdd)
            {
                dbPermission.EntityDefinition = dbEntityDefinitions.GetValueOrDefault(entityPermissionToAdd.EntityDefinition.EntityDefinitionCode);

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

        if (!newEntityCodes.Any())
        {
            return;
        }

        foreach (var definitionCode in newEntityCodes)
        {
            // get the entity definition class
            var entityDefinition = _entityDefinitionRepository.GetRequiredByCode(definitionCode);

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
        bool allowDeletions
        )
    {
        // Super admin role does not require any db-based permissions.
        if (roleDefinition is SuperAdminRole)
        {
            return;
        }

        var roleInitializer = _rolePermissionInitializerFactory.Create(roleDefinition);
        var permissionSetBuilder = _permissionSetBuilderFactory.Create(codePermissions);
        roleInitializer.Initialize(permissionSetBuilder);

        var permissionsToInclude = permissionSetBuilder.Build();

        // Remove permissions
        if (allowDeletions)
        {
            var permissionsToRemove = dbRole
                .RolePermissions
                .Where(p => !permissionsToInclude.Any(i => i.GetUniqueIdentifier() == p.Permission.GetUniqueCode()))
                .ToList();

            foreach (var permissonToRemove in permissionsToRemove)
            {
                dbRole.RolePermissions.Remove(permissonToRemove);
            }
        }

        if (permissionsToInclude.Count == 0)
        {
            return;
        }

        ValidatePermissions(dbRole, permissionsToInclude);

        // add new permissions
        IEnumerable<IPermission> permissionsToAdd;

        if (dbRole.RolePermissions.Count == 0)
        {
            permissionsToAdd = permissionsToInclude;
        }
        else
        {
            permissionsToAdd = permissionsToInclude
                .Where(i => !dbRole.RolePermissions.Any(p => p.Permission.GetUniqueCode() == i.GetUniqueIdentifier()));
        }

        foreach (var permissionToAdd in permissionsToAdd)
        {
            var uniquePermissionCode = permissionToAdd.GetUniqueIdentifier();
            var dbPermission = dbPermissions.GetValueOrDefault(uniquePermissionCode);

            if (dbPermission == null)
            {
                throw new Exception("dbPermissions lookup does not contain the specified permission, but was expected: " + uniquePermissionCode);
            }

            dbRole.RolePermissions.Add(new RolePermission
            {
                Permission = dbPermission
            });
        }
    }

    /// <summary>
    /// Validation ensures that we don't have any entity permissions
    /// that have elevated access without first being granted a read 
    /// permission. E.g. having 'UpdatePage' permission without also
    /// having 'ReadPage' permission.
    /// </summary>
    private void ValidatePermissions(Role dbRole, IEnumerable<IPermission> permissions)
    {
        var existingPermissions = dbRole.RolePermissions;
        var entityWithoutReadPermission = permissions
            .FilterToEntityPermissions()
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
                var msg = $"Could not update {dbRole.Title} role with additional permissions for entity '{entityCode}' because the role does not have read permissions to the entity.";
                throw new ValidationException(msg);
            }
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

    private static void ValidateRole(IReadOnlyCollection<Role> existingRoles, IRoleDefinition roleDefinition)
    {
        if (existingRoles.Any(r =>
            r.Title.Equals(roleDefinition.Title?.Trim(), StringComparison.OrdinalIgnoreCase)
            && r.UserAreaCode == roleDefinition.UserAreaCode)
            )
        {
            throw new UniqueConstraintViolationException($"A role with the title '{roleDefinition.Title}' already exists", nameof(Role.Title));
        }
    }

    private async Task EnsureUserAreaExistsAndValidatePermissionAsync(
        IReadOnlyCollection<Role> existingRoles,
        IExecutionContext executionContext
        )
    {
        var allUserAreaCodes = _roleDefinitionRepository
            .GetAll()
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

    public IEnumerable<IPermissionApplication> GetPermissions(RegisterPermissionsAndRolesCommand command)
    {
        yield return new RoleCreatePermission();
    }
}
