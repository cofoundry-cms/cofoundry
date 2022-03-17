using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class RoleMicroSummaryMapper : IRoleMicroSummaryMapper
{
    private readonly IUserAreaDefinitionRepository _userAreaRepository;

    public RoleMicroSummaryMapper(
        IUserAreaDefinitionRepository userAreaRepository
        )
    {
        _userAreaRepository = userAreaRepository;
    }

    public virtual RoleMicroSummary Map(Role dbRole)
    {
        if (dbRole == null) return null;

        var role = new RoleMicroSummary()
        {
            RoleId = dbRole.RoleId,
            Title = dbRole.Title
        };

        var userArea = _userAreaRepository.GetRequiredByCode(dbRole.UserAreaCode);
        role.UserArea = new UserAreaMicroSummary()
        {
            UserAreaCode = dbRole.UserAreaCode,
            Name = userArea.Name
        };

        return role;
    }

    public RoleMicroSummary Map(RoleDetails roleDetails)
    {
        if (roleDetails == null) return null;

        return new RoleMicroSummary()
        {
            RoleId = roleDetails.RoleId,
            Title = roleDetails.Title,
            UserArea = roleDetails.UserArea
        };
    }
}
