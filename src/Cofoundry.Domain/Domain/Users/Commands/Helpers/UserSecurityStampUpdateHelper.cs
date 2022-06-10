using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class UserSecurityStampUpdateHelper : IUserSecurityStampUpdateHelper
{
    private readonly ISecurityStampGenerator _securityStampGenerator;
    private readonly IUserSessionService _userSessionService;
    private readonly IMessageAggregator _messageAggregator;

    public UserSecurityStampUpdateHelper(
        ISecurityStampGenerator securityStampGenerator,
        IUserSessionService userSessionService,
        IMessageAggregator messageAggregator
        )
    {
        _securityStampGenerator = securityStampGenerator;
        _userSessionService = userSessionService;
        _messageAggregator = messageAggregator;
    }

    public void Update(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        user.SecurityStamp = _securityStampGenerator.Generate();
    }

    public async Task OnTransactionCompleteAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var userAreaCode = user.UserArea != null ? user.UserArea.UserAreaCode : user.UserAreaCode;

        if (string.IsNullOrEmpty(userAreaCode))
        {
            throw new ArgumentException("User is not assigned to a user area.", nameof(user));
        }

        if (user.UserId < 1)
        {
            throw new ArgumentException("User has not been saved.", nameof(user));
        }

        await _messageAggregator.PublishAsync(new UserSecurityStampUpdatedMessage()
        {
            UserAreaCode = user.UserAreaCode,
            UserId = user.UserId
        });

        await _userSessionService.RefreshAsync(user.UserAreaCode, user.UserId);
    }
}
