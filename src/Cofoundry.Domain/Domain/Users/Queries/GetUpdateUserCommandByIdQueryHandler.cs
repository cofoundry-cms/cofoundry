using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetUpdateUserCommandByIdQueryHandler
    : IQueryHandler<GetPatchableCommandByIdQuery<UpdateUserCommand>, UpdateUserCommand>
    , ISignedInPermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

    public GetUpdateUserCommandByIdQueryHandler(
        CofoundryDbContext dbContext,
        IPermissionValidationService permissionValidationService,
        IUserAreaDefinitionRepository userAreaDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _permissionValidationService = permissionValidationService;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
    }

    public async Task<UpdateUserCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdateUserCommand> query, IExecutionContext executionContext)
    {
        var dbUser = await _dbContext
            .Users
            .AsNoTracking()
            .FilterNotDeleted()
            .FilterNotSystemAccount()
            .FilterById(query.Id)
            .SingleOrDefaultAsync();

        if (dbUser == null) return null;

        if (dbUser.UserAreaCode == CofoundryAdminUserArea.Code)
        {
            _permissionValidationService.EnforceCurrentUserOrHasPermission<CofoundryUserReadPermission>(query.Id, executionContext.UserContext);
        }
        else
        {
            _permissionValidationService.EnforceCurrentUserOrHasPermission<NonCofoundryUserReadPermission>(query.Id, executionContext.UserContext);
        }

        var userArea = _userAreaDefinitionRepository.GetByCode(dbUser.UserAreaCode);

        var user = new UpdateUserCommand()
        {
            Email = dbUser.Email,
            FirstName = dbUser.FirstName,
            LastName = dbUser.LastName,
            RequirePasswordChange = dbUser.RequirePasswordChange,
            RoleId = dbUser.RoleId,
            UserId = dbUser.UserId,
            IsAccountVerified = dbUser.AccountVerifiedDate.HasValue,
            IsActive = !dbUser.DeactivatedDate.HasValue
        };

        if (!userArea.UseEmailAsUsername)
        {
            user.Username = dbUser.Username;
        }

        return user;
    }
}
