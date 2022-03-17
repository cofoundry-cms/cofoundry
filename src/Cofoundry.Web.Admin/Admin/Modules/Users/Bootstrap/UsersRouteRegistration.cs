using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web.Admin;

public class UsersRouteRegistration : IOrderedRouteRegistration
{
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

    public UsersRouteRegistration(
        IUserAreaDefinitionRepository userAreaDefinitionRepository
        )
    {
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
    }

    public int Ordering => (int)RouteRegistrationOrdering.Early;

    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        foreach (var userArea in _userAreaDefinitionRepository.GetAll())
        {
            var routePrefix = SlugFormatter.ToSlug(userArea.Name);

            routeBuilder
                .ForAdminController<UsersModuleController>(routePrefix + "-users")
                .MapIndexRoute(new { UserArea = userArea });
        }
    }
}
