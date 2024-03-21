using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IRoleMicroSummaryMapper"/>.
/// </summary>
public class RoleMicroSummaryMapper : IRoleMicroSummaryMapper
{
    private readonly IUserAreaDefinitionRepository _userAreaRepository;

    public RoleMicroSummaryMapper(
        IUserAreaDefinitionRepository userAreaRepository
        )
    {
        _userAreaRepository = userAreaRepository;
    }

    /// <inheritdoc/>
    public virtual RoleMicroSummary? Map(Role? dbRole)
    {
        if (dbRole == null)
        {
            return null;
        }

        var userArea = _userAreaRepository.GetRequiredByCode(dbRole.UserAreaCode);
        var role = new RoleMicroSummary()
        {
            RoleId = dbRole.RoleId,
            Title = dbRole.Title,
            UserArea = new UserAreaMicroSummary()
            {
                UserAreaCode = dbRole.UserAreaCode,
                Name = userArea.Name
            }
        };

        return role;
    }

    /// <inheritdoc/>
    public RoleMicroSummary? Map(RoleDetails? roleDetails)
    {
        if (roleDetails == null)
        {
            return null;
        }

        return new RoleMicroSummary()
        {
            RoleId = roleDetails.RoleId,
            Title = roleDetails.Title,
            UserArea = roleDetails.UserArea
        };
    }
}
