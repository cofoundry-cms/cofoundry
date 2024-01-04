using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageRenderDetailsMapper"/>.
/// </summary>
public class PageRenderDetailsMapper : IPageRenderDetailsMapper
{
    private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;
    private readonly IPageRenderSummaryMapper _pageRenderSummaryMapper;

    public PageRenderDetailsMapper(
        IPageTemplateMicroSummaryMapper pageTemplateMapper,
        IPageRenderSummaryMapper pageRenderSummaryMapper
        )
    {
        _pageTemplateMapper = pageTemplateMapper;
        _pageRenderSummaryMapper = pageRenderSummaryMapper;
    }

    /// <inheritdoc/>
    public virtual PageRenderDetails Map(
        PageVersion dbPageVersion,
        PageRoute pageRoute
        )
    {
        ArgumentNullException.ThrowIfNull(dbPageVersion);
        ArgumentNullException.ThrowIfNull(pageRoute);

        var page = _pageRenderSummaryMapper.Map<PageRenderDetails>(dbPageVersion, pageRoute);

        MapInternal(dbPageVersion, page);

        return page;
    }

    /// <inheritdoc/>
    public virtual PageRenderDetails Map(PageVersion dbPageVersion, IReadOnlyDictionary<int, PageRoute> pageRouteLookup)
    {
        ArgumentNullException.ThrowIfNull(dbPageVersion);
        ArgumentNullException.ThrowIfNull(pageRouteLookup);

        var page = _pageRenderSummaryMapper.Map<PageRenderDetails>(dbPageVersion, pageRouteLookup);

        MapInternal(dbPageVersion, page);

        return page;
    }

    protected void MapInternal(PageVersion dbPageVersion, PageRenderDetails page)
    {
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
    }
}
