using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web.Admin;

public class CustomEntitiesRouteRegistration : IOrderedRouteRegistration
{
    private readonly IEnumerable<ICustomEntityDefinition> _customEntityDefinition;

    public CustomEntitiesRouteRegistration(
        IEnumerable<ICustomEntityDefinition> customEntityDefinition
        )
    {
        _customEntityDefinition = customEntityDefinition;
    }

    public int Ordering => (int)RouteRegistrationOrdering.Early;

    public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
    {
        foreach (var definition in _customEntityDefinition)
        {
            var routePrefix = SlugFormatter.ToSlug(definition.NamePlural);

            routeBuilder
                .ForAdminController<CustomEntityModuleController>(routePrefix)
                .MapIndexRoute(new { Definition = definition });
        }
    }
}
