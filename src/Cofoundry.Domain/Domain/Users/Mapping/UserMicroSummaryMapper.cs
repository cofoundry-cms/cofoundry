using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class UserMicroSummaryMapper : IUserMicroSummaryMapper
{
    private readonly IUserAreaDefinitionRepository _userAreaRepository;

    public UserMicroSummaryMapper(
        IUserAreaDefinitionRepository userAreaRepository
        )
    {
        _userAreaRepository = userAreaRepository;
    }

    [return: NotNullIfNotNull(nameof(dbUser))]
    public virtual UserMicroSummary? Map(User? dbUser)
    {
        if (dbUser == null)
        {
            return null;
        }

        var userArea = _userAreaRepository.GetRequiredByCode(dbUser.UserAreaCode);
        EntityNotFoundException.ThrowIfNull(userArea, dbUser.UserAreaCode);

        var user = new UserMicroSummary()
        {
            UserId = dbUser.UserId,
            DisplayName = dbUser.DisplayName,
            AccountStatus = UserAccountStatusMapper.Map(dbUser),
            UserArea = new()
            {
                UserAreaCode = dbUser.UserAreaCode,
                Name = userArea.Name
            }
        };

        return user;
    }
}
