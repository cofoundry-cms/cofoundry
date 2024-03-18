﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Updates the password of the currently logged in user, using the
/// OldPassword field to authenticate the request.
/// </summary>
public class UpdateUserPasswordByCredentialsCommandHandler
    : ICommandHandler<UpdateUserPasswordByCredentialsCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDomainRepository _domainRepository;
    private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;

    public UpdateUserPasswordByCredentialsCommandHandler(
        CofoundryDbContext dbContext,
        IDomainRepository domainRepository,
        IPasswordUpdateCommandHelper passwordUpdateCommandHelper
        )
    {
        _dbContext = dbContext;
        _domainRepository = domainRepository;
        _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
    }

    public async Task ExecuteAsync(UpdateUserPasswordByCredentialsCommand command, IExecutionContext executionContext)
    {
        if (IsLoggedInAlready(command, executionContext))
        {
            throw new Exception($"{nameof(UpdateUserPasswordByCredentialsCommand)} cannot be used when the user is already logged in.");
        }

        var authResult = await GetUserSignInInfoAsync(command, executionContext);
        authResult.ThrowIfNotSuccess();
        EntityInvalidOperationException.ThrowIfNull(authResult, authResult.User);

        var user = await GetUserAsync(authResult.User);
        var updatePasswordCommand = new UpdateUserPasswordByUserIdCommand()
        {
            UserId = authResult.User.UserId,
            NewPassword = command.NewPassword
        };

        using (var scope = _domainRepository.Transactions().CreateScope())
        {
            await _domainRepository
                .WithElevatedPermissions()
                .ExecuteCommandAsync(updatePasswordCommand);
            await _passwordUpdateCommandHelper.SendPasswordChangedNotification(user);

            await scope.CompleteAsync();
        }

        // We pass out the userid since we do the auth inside the command and it might be useful to the callee
        command.OutputUserId = authResult.User.UserId;
    }

    private async Task<User> GetUserAsync(UserSignInInfo userSignInInfo)
    {
        // in most other command that send password changed notifications we already have a user to 
        // send on to the helper, but not here
        var user = await _dbContext
            .Users
            .AsNoTracking()
            .IncludeForSummary()
            .FilterCanSignIn()
            .FilterById(userSignInInfo.UserId)
            .SingleOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(user, userSignInInfo.UserId);

        return user;
    }

    private Task<UserCredentialsAuthenticationResult> GetUserSignInInfoAsync(
        UpdateUserPasswordByCredentialsCommand command,
        IExecutionContext executionContext
        )
    {
        return _domainRepository
            .WithContext(executionContext)
            .ExecuteQueryAsync(new AuthenticateUserCredentialsQuery()
            {
                UserAreaCode = command.UserAreaCode,
                Username = command.Username,
                Password = command.OldPassword,
                PropertyToValidate = nameof(command.OldPassword)
            });
    }

    private static bool IsLoggedInAlready(UpdateUserPasswordByCredentialsCommand command, IExecutionContext executionContext)
    {
        var currentContext = executionContext.UserContext;
        var isLoggedIntoDifferentUserArea = currentContext.UserArea?.UserAreaCode != command.UserAreaCode;

        return currentContext.UserId.HasValue && !isLoggedIntoDifferentUserArea;
    }
}
