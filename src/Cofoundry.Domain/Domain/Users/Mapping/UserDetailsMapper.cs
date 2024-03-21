using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class UserDetailsMapper : IUserDetailsMapper
{
    private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;
    private readonly IRoleDetailsMapper _roleDetailsMapper;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

    public UserDetailsMapper(
        IUserMicroSummaryMapper userMicroSummaryMapper,
        IRoleDetailsMapper roleDetailsMapper,
        IUserAreaDefinitionRepository userAreaDefinitionRepository
        )
    {
        _userMicroSummaryMapper = userMicroSummaryMapper;
        _roleDetailsMapper = roleDetailsMapper;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
    }

    [return: NotNullIfNotNull(nameof(dbUser))]
    public virtual UserDetails? Map(User? dbUser)
    {
        if (dbUser == null)
        {
            return null;
        }

        MissingIncludeException.ThrowIfNull(dbUser, u => u.Role);

        var role = _roleDetailsMapper.Map(dbUser.Role);
        EntityNotFoundException.ThrowIfNull(role, dbUser.Role.RoleId);

        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(dbUser.UserAreaCode);
        EntityNotFoundException.ThrowIfNull(userArea, dbUser.UserAreaCode);

        var user = new UserDetails()
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
            LastPasswordChangeDate = dbUser.LastPasswordChangeDate,
            RequirePasswordChange = dbUser.RequirePasswordChange,
            AccountVerifiedDate = dbUser.AccountVerifiedDate,
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
