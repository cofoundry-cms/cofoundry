using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageRenderDetailsMapper"/>.
/// </summary>
public class PageRenderDetailsMapper : IPageRenderDetailsMapper
{
    private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;
    private readonly IPageRenderSummaryMapper _pageRenderSummaryMapper;
    private readonly IOpenGraphDataMapper _openGraphDataMapper;

    public PageRenderDetailsMapper(
        IPageTemplateMicroSummaryMapper pageTemplateMapper,
        IPageRenderSummaryMapper pageRenderSummaryMapper,
        IOpenGraphDataMapper openGraphDataMapper
        )
    {
        _pageTemplateMapper = pageTemplateMapper;
        _pageRenderSummaryMapper = pageRenderSummaryMapper;
        _openGraphDataMapper = openGraphDataMapper;
    }

    /// <inheritdoc/>
    public virtual PageRenderDetails Map(
        PageVersion dbPageVersion,
        PageRoute pageRoute
        )
    {
        ArgumentNullException.ThrowIfNull(dbPageVersion);
        ArgumentNullException.ThrowIfNull(pageRoute);

        var page = MapInternal(dbPageVersion);
        page.PageRoute = pageRoute;

        return page;
    }

    /// <inheritdoc/>
    public virtual PageRenderDetails Map(
        PageVersion dbPageVersion,
        IReadOnlyDictionary<int, PageRoute> pageRouteLookup
        )
    {
        ArgumentNullException.ThrowIfNull(dbPageVersion);
        ArgumentNullException.ThrowIfNull(pageRouteLookup);

        var page = MapInternal(dbPageVersion);

        var pageRoute = pageRouteLookup.GetValueOrDefault(page.PageId);
        if (pageRoute == null)
        {
            throw new Exception($"Unable to locate a page route when mapping a {nameof(PageRenderSummary)} with an id of {page.PageId}.");
        }

        page.PageRoute = pageRoute;

        return page;
    }

    protected PageRenderDetails MapInternal(PageVersion dbPageVersion)
    {
        var page = new PageRenderDetails()
        {
            MetaDescription = dbPageVersion.MetaDescription,
            PageId = dbPageVersion.PageId,
            PageVersionId = dbPageVersion.PageVersionId,
            Title = dbPageVersion.Title,
            WorkFlowStatus = (WorkFlowStatus)dbPageVersion.WorkFlowStatusId,
            CreateDate = dbPageVersion.CreateDate
        };

        page.OpenGraph = _openGraphDataMapper.Map(dbPageVersion);
        page.Template = _pageTemplateMapper.Map(dbPageVersion.PageTemplate);
        page.Regions = dbPageVersion
            .PageTemplate
            .PageTemplateRegions
            .Select(r => new PageRegionRenderDetails()
            {
                PageTemplateRegionId = r.PageTemplateRegionId,
                Name = r.Name
                // Blocks mapped elsewhere
            })
            .ToArray();

        return page;
    }
}
