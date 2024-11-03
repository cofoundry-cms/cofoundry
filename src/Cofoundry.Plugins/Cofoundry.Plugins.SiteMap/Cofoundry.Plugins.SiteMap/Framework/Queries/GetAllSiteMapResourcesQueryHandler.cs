namespace Cofoundry.Plugins.SiteMap;

public class GetAllSiteMapResourcesQueryHandler
    : IQueryHandler<GetAllSiteMapResourcesQuery, IReadOnlyCollection<ISiteMapResource>>
    , IIgnorePermissionCheckHandler
{
    private readonly IEnumerable<ISiteMapResourceRegistration> _siteMapRegistrations;
    private readonly IEnumerable<IAsyncSiteMapResourceRegistration> _asyncSiteMapRegistrations;

    public GetAllSiteMapResourcesQueryHandler(
        IEnumerable<ISiteMapResourceRegistration> siteMapRegistrations,
        IEnumerable<IAsyncSiteMapResourceRegistration> asyncSiteMapRegistrations
        )
    {
        _siteMapRegistrations = siteMapRegistrations;
        _asyncSiteMapRegistrations = asyncSiteMapRegistrations;
    }

    public async Task<IReadOnlyCollection<ISiteMapResource>> ExecuteAsync(GetAllSiteMapResourcesQuery query, IExecutionContext executionContext)
    {
        var allResources = new List<ISiteMapResource>();

        foreach (var registration in _siteMapRegistrations)
        {
            var resources = registration.GetResources();
            allResources.AddRange(resources);
        }

        foreach (var registration in _asyncSiteMapRegistrations)
        {
            var resources = await registration.GetResourcesAsync();
            allResources.AddRange(resources);
        }

        return allResources;
    }
}
