﻿using Cofoundry.Domain.Data;
using Microsoft.AspNetCore.Identity;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Updates the password of the currently logged in user, using the
/// <see cref="UpdateCurrentUserPasswordCommand.OldPassword"/> field 
/// to authenticate the request.
/// </summary>
public class UpdateCurrentUserPasswordCommandHandler
    : ICommandHandler<UpdateCurrentUserPasswordCommand>
    , IPermissionRestrictedCommandHandler<UpdateCurrentUserPasswordCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDomainRepository _domainRepository;
    private readonly UserAuthenticationHelper _userAuthenticationHelper;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly IUserAreaDefinitionRepository _userAreaRepository;
    private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
    private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
    private readonly IUserContextCache _userContextCache;
    private readonly IPasswordPolicyService _newPasswordValidationService;
    private readonly IMessageAggregator _messageAggregator;

    public UpdateCurrentUserPasswordCommandHandler(
        CofoundryDbContext dbContext,
        IDomainRepository domainRepository,
        UserAuthenticationHelper userAuthenticationHelper,
        IPermissionValidationService permissionValidationService,
        IUserAreaDefinitionRepository userAreaRepository,
        IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
        IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
        IUserContextCache userContextCache,
        IPasswordPolicyService newPasswordValidationService,
        IMessageAggregator messageAggregator
        )
    {
        _dbContext = dbContext;
        _domainRepository = domainRepository;
        _userAuthenticationHelper = userAuthenticationHelper;
        _permissionValidationService = permissionValidationService;
        _userAreaRepository = userAreaRepository;
        _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
        _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
        _userContextCache = userContextCache;
        _newPasswordValidationService = newPasswordValidationService;
        _messageAggregator = messageAggregator;
    }

    public async Task ExecuteAsync(UpdateCurrentUserPasswordCommand command, IExecutionContext executionContext)
    {
        _permissionValidationService.EnforceIsSignedIn(executionContext.UserContext);

        var user = await GetUser(executionContext);
        EntityNotFoundException.ThrowIfNull(user, executionContext.UserContext.UserId);

        await ValidateMaxAuthenticationAttemptsNotExceededAsync(user, executionContext);
        await AuthenticateAsync(command, user);
        await ValidatePasswordAsync(command, user, executionContext);

        _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, user, executionContext);
        _userSecurityStampUpdateHelper.Update(user);

        using (var scope = _domainRepository.Transactions().CreateScope())
        {
            await _dbContext.SaveChangesAsync();
            await _domainRepository
                .WithContext(executionContext)
                .ExecuteCommandAsync(new InvalidateAuthorizedTaskBatchCommand(user.UserId, UserAccountRecoveryAuthorizedTaskType.Code));

            await _passwordUpdateCommandHelper.SendPasswordChangedNotification(user);

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

    private Task<User?> GetUser(IExecutionContext executionContext)
    {
        EntityInvalidOperationException.ThrowIfNull(executionContext.UserContext, executionContext.UserContext.UserId);

        return _dbContext
            .Users
            .IncludeForSummary()
            .FilterCanSignIn()
            .FilterById(executionContext.UserContext.UserId.Value)
            .SingleOrDefaultAsync();
    }

    private async Task ValidateMaxAuthenticationAttemptsNotExceededAsync(User dbUser, IExecutionContext executionContext)
    {
        var query = new HasExceededMaxAuthenticationAttemptsQuery()
        {
            UserAreaCode = dbUser.UserAreaCode,
            Username = dbUser.Username
        };

        var hasExceededMaxAuthenticationAttempts = await _domainRepository
            .WithContext(executionContext)
            .ExecuteQueryAsync(query);

        if (hasExceededMaxAuthenticationAttempts)
        {
            UserValidationErrors.Authentication.TooManyFailedAttempts.Throw();
        }
    }

    private async Task ValidatePasswordAsync(UpdateCurrentUserPasswordCommand command, User user, IExecutionContext executionContext)
    {
        var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
        _passwordUpdateCommandHelper.ValidateUserArea(userArea);

        var context = NewPasswordValidationContext.MapFromUser(user, executionContext);
        context.CurrentPassword = command.OldPassword;
        context.Password = command.NewPassword;
        context.PropertyName = nameof(command.NewPassword);

        await _newPasswordValidationService.ValidateAsync(context);
    }

    private async Task AuthenticateAsync(UpdateCurrentUserPasswordCommand command, User user)
    {
        if (_userAuthenticationHelper.VerifyPassword(user, command.OldPassword) == PasswordVerificationResult.Failed)
        {
            var logFailedAttemptCommand = new LogFailedAuthenticationAttemptCommand(user.UserAreaCode, user.Username);
            await _domainRepository.ExecuteCommandAsync(logFailedAttemptCommand);

            UserValidationErrors.Authentication.InvalidPassword.Throw(nameof(command.OldPassword));
        }
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdateCurrentUserPasswordCommand command)
    {
        yield return new CurrentUserUpdatePermission();
    }
}
