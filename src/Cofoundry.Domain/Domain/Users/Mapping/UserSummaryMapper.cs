using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class UserSummaryMapper : IUserSummaryMapper
{
    private readonly IUserAreaDefinitionRepository _userAreaRepository;
    private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;
    private readonly IRoleMicroSummaryMapper _roleMicroSummaryMapper;

    public UserSummaryMapper(
        IUserAreaDefinitionRepository userAreaRepository,
        IUserMicroSummaryMapper userMicroSummaryMapper,
        IRoleMicroSummaryMapper roleMicroSummaryMapper
        )
    {
        _userAreaRepository = userAreaRepository;
        _userMicroSummaryMapper = userMicroSummaryMapper;
        _roleMicroSummaryMapper = roleMicroSummaryMapper;
    }

    [return: NotNullIfNotNull(nameof(dbUser))]
    public virtual UserSummary? Map(User? dbUser)
    {
        if (dbUser == null)
        {
            return null;
        }

        MissingIncludeException.ThrowIfNull(dbUser, u => u.Role);

        var role = _roleMicroSummaryMapper.Map(dbUser.Role);
        EntityNotFoundException.ThrowIfNull(role, dbUser.Role.RoleId);

        var userArea = _userAreaRepository.GetRequiredByCode(dbUser.UserAreaCode);
        EntityNotFoundException.ThrowIfNull(userArea, dbUser.UserAreaCode);

        var user = new UserSummary()
        {
            UserId = dbUser.UserId,
            DisplayName = dbUser.DisplayName,
            AccountStatus = UserAccountStatusMapper.Map(dbUser),
            UserArea = new()
            {
                UserAreaCode = dbUser.UserAreaCode,
                Name = userArea.Name
            },
            Email = dbUser.Email,
            FirstName = dbUser.FirstName,
            LastName = dbUser.LastName,
            Username = dbUser.Username,
            LastSignInDate = dbUser.LastSignInDate,
            AuditData = new CreateAuditData()
            {
                CreateDate = dbUser.CreateDate,
                Creator = _userMicroSummaryMapper.Map(dbUser.Creator)
            },
            Role = role
        };

        return user;
    }
}
