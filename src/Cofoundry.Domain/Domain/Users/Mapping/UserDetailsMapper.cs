using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class UserDetailsMapper : IUserDetailsMapper
{
    private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;
    private readonly IRoleDetailsMapper _roleDetailsMapper;

    public UserDetailsMapper(
        IUserMicroSummaryMapper userMicroSummaryMapper,
        IRoleDetailsMapper roleDetailsMapper
        )
    {
        _userMicroSummaryMapper = userMicroSummaryMapper;
        _roleDetailsMapper = roleDetailsMapper;
    }

    public virtual UserDetails Map(User dbUser)
    {
        if (dbUser == null) return null;

        if (dbUser.Role == null)
        {
            throw new ArgumentException("dbUser.Role must be included in the query to map to use the UserDetailsMapper");
        }

        var user = _userMicroSummaryMapper.Map<UserDetails>(dbUser);
        user.Email = dbUser.Email;
        user.FirstName = dbUser.FirstName;
        user.LastName = dbUser.LastName;
        user.Username = dbUser.Username;
        user.LastSignInDate = dbUser.LastSignInDate;
        user.LastPasswordChangeDate = dbUser.LastPasswordChangeDate;
        user.RequirePasswordChange = dbUser.RequirePasswordChange;
        user.AccountVerifiedDate = dbUser.AccountVerifiedDate;

        user.AuditData = new CreateAuditData()
        {
            CreateDate = dbUser.CreateDate
        };

        if (dbUser.Creator != null)
        {
            user.AuditData.Creator = _userMicroSummaryMapper.Map(dbUser.Creator);
        }

        user.Role = _roleDetailsMapper.Map(dbUser.Role);

        return user;
    }
}
