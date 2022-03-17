using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Updates the password of a user to a specific value. Generally a user shouldn't
/// be able set another users password explicity, but this command is provided for
/// scenarios where authorization happpens through another mechanism such as via
/// <see cref="UpdateUserPasswordByCredentialsCommand"/>.
/// </summary>
public class UpdateUserPasswordByUserIdCommandHandler
    : ICommandHandler<UpdateUserPasswordByUserIdCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDomainRepository _domainRepository;
    private readonly IUserAreaDefinitionRepository _userAreaRepository;
    private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
    private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
    private readonly IUserContextCache _userContextCache;
    private readonly IPasswordPolicyService _newPasswordValidationService;
    private readonly IMessageAggregator _messageAggregator;

    public UpdateUserPasswordByUserIdCommandHandler(
        CofoundryDbContext dbContext,
        IDomainRepository domainRepository,
        IUserAreaDefinitionRepository userAreaRepository,
        IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
        IPermissionValidationService permissionValidationService,
        UserCommandPermissionsHelper userCommandPermissionsHelper,
        IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
        IUserContextCache userContextCache,
        IPasswordPolicyService newPasswordValidationService,
        IMessageAggregator messageAggregator
        )
    {
        _dbContext = dbContext;
        _domainRepository = domainRepository;
        _userAreaRepository = userAreaRepository;
        _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
        _permissionValidationService = permissionValidationService;
        _userCommandPermissionsHelper = userCommandPermissionsHelper;
        _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
        _userContextCache = userContextCache;
        _newPasswordValidationService = newPasswordValidationService;
        _messageAggregator = messageAggregator;
    }

    public async Task ExecuteAsync(UpdateUserPasswordByUserIdCommand command, IExecutionContext executionContext)
    {
        ValidatePermissions(executionContext);

        var user = await GetUserAsync(command.UserId);
        await ValidatePasswordAsync(command, user, executionContext);

        _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, user, executionContext);
        _userSecurityStampUpdateHelper.Update(user);

        using (var scope = _domainRepository.Transactions().CreateScope())
        {
            await _dbContext.SaveChangesAsync();

            // Typically we only invalidate when the current user changes their password, but we
            // can't be certain of the origin of the request here, so invalidate them anyway.
            await _domainRepository
                .WithContext(executionContext)
                .ExecuteCommandAsync(new InvalidateAuthorizedTaskBatchCommand(user.UserId, UserAccountRecoveryAuthorizedTaskType.Code));

            scope.QueueCompletionTask(() => OnTransactionComplete(user));
            await scope.CompleteAsync();
        }
    }

    private async Task OnTransactionComplete(User user)
    {
        _userContextCache.Clear(user.UserId);

        await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);

        await _messageAggregator.PublishAsync(new UserPasswordUpdatedMessage()
        {
            UserAreaCode = user.UserAreaCode,
            UserId = user.UserId
        });
    }

    private async Task<User> GetUserAsync(int userId)
    {
        var user = await _dbContext
            .Users
            .FilterById(userId)
            .FilterCanSignIn()
            .SingleOrDefaultAsync();

        EntityNotFoundException.ThrowIfNull(user, userId);

        return user;
    }

    public void ValidatePermissions(IExecutionContext executionContext)
    {
        if (executionContext.UserContext.IsCofoundryUser())
        {
            _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
        }
        else
        {
            _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
        }
    }

    private async Task ValidatePasswordAsync(
        UpdateUserPasswordByUserIdCommand command,
        User user,
        IExecutionContext executionContext
        )
    {
        await _userCommandPermissionsHelper.ThrowIfCannotManageSuperAdminAsync(user, executionContext);

        var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
        _passwordUpdateCommandHelper.ValidateUserArea(userArea);
        _passwordUpdateCommandHelper.ValidatePermissions(userArea, executionContext);

        var context = NewPasswordValidationContext.MapFromUser(user);
        context.Password = command.NewPassword;
        context.PropertyName = nameof(command.NewPassword);
        context.ExecutionContext = executionContext;

        await _newPasswordValidationService.ValidateAsync(context);
    }
}
