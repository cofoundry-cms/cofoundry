namespace Cofoundry.Domain.Internal;

/// <summary>
/// Signs out the user currently logged into the "ambient" scheme, ending the session.
/// The ambient scheme usually represents the default user area, unless it has been switched
/// during the request. A <see cref="UserSignedOutMessage"/> is published once the user is 
/// signed out; if the user is not signed in, no action is taken.
/// </summary>
public class SignOutCurrentUserCommandHandler
    : ICommandHandler<SignOutCurrentUserCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly IDomainRepository _domainRepository;
    private readonly IUserSessionService _userSessionService;
    private readonly IMessageAggregator _messageAggregator;

    public SignOutCurrentUserCommandHandler(
        IDomainRepository domainRepository,
        IUserSessionService userSessionService,
        IMessageAggregator messageAggregator
        )
    {
        _domainRepository = domainRepository;
        _userSessionService = userSessionService;
        _messageAggregator = messageAggregator;
    }


    public async Task ExecuteAsync(SignOutCurrentUserCommand command, IExecutionContext executionContext)
    {
        var userAreaCode = command.UserAreaCode;
        int? userId;

        if (string.IsNullOrEmpty(command.UserAreaCode))
        {
            userId = executionContext.UserContext.UserId;
            userAreaCode = executionContext.UserContext.UserArea?.UserAreaCode;
        }
        else
        {
            userId = await _userSessionService.GetUserIdByUserAreaCodeAsync(command.UserAreaCode);
        }

        if (userAreaCode == null)
        {
            // User not logged in, no action
            return;
        }

        await _userSessionService.SignOutAsync(userAreaCode);

        if (userId.HasValue)
        {
            await _domainRepository.Transactions().QueueCompletionTaskAsync(() => OnTransactionComplete(userAreaCode, userId));
        }
    }

    private async Task OnTransactionComplete(string userAreaCode, int? userId)
    {
        await _messageAggregator.PublishAsync(new UserSignedOutMessage()
        {
            UserId = userId.Value,
            UserAreaCode = userAreaCode
        });
    }
}
