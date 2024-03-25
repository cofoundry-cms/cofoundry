using Microsoft.AspNetCore.Routing;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IRouteInitializer"/>.
/// </summary>
public class RouteInitializer : IRouteInitializer
{
    private readonly IEnumerable<IRouteRegistration> _routeRegistrations;

    public RouteInitializer(
        IEnumerable<IRouteRegistration> routeRegistrations
        )
    {
        _routeRegistrations = routeRegistrations;
    }

    /// <inheritdoc/>
    public void Initialize(IEndpointRouteBuilder routes)
    {
        IReadOnlyCollection<IRouteRegistration> sortedRoutes;

        try
        {
            // Then do a Topological Sort based on dependencies
            sortedRoutes = OrderableTaskSorter.Sort(_routeRegistrations);
        }
        catch (CyclicDependencyException ex)
        {
            throw new CyclicDependencyException($"A cyclic dependency has been detected between multiple {nameof(IRouteRegistration)} classes. Check your route registrations to ensure they do not depend on each other. For more details see the inner exception message.", ex);
        }

        foreach (var routeRegistration in sortedRoutes)
        {
            routeRegistration.RegisterRoutes(routes);
        }
    }
}
