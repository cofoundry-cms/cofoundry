using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageRenderSummaryMapper"/>.
/// </summary>
public class PageRenderSummaryMapper : IPageRenderSummaryMapper
{
    private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;
    private readonly IOpenGraphDataMapper _openGraphDataMapper;

    public PageRenderSummaryMapper(
        IPageTemplateMicroSummaryMapper pageTemplateMapper,
        IOpenGraphDataMapper openGraphDataMapper
        )
    {
        _pageTemplateMapper = pageTemplateMapper;
        _openGraphDataMapper = openGraphDataMapper;
    }

    /// <inheritdoc/>
    public virtual PageRenderSummary Map(PageVersion dbPageVersion, PageRoute pageRoute)
    {
        ArgumentNullException.ThrowIfNull(dbPageVersion);
        ArgumentNullException.ThrowIfNull(pageRoute);

        var page = MapInternal(dbPageVersion);
        page.PageRoute = pageRoute;

        return page;
    }

    /// <inheritdoc/>
    public virtual PageRenderSummary Map(PageVersion dbPageVersion, IReadOnlyDictionary<int, PageRoute> pageRouteLookup)
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

    protected PageRenderSummary MapInternal(PageVersion dbPageVersion)
    {
        var page = new PageRenderSummary()
        {
            MetaDescription = dbPageVersion.MetaDescription,
            PageId = dbPageVersion.PageId,
            PageVersionId = dbPageVersion.PageVersionId,
            Title = dbPageVersion.Title,
            WorkFlowStatus = (WorkFlowStatus)dbPageVersion.WorkFlowStatusId,
            CreateDate = dbPageVersion.CreateDate
        };

        page.OpenGraph = _openGraphDataMapper.Map(dbPageVersion);

        return page;
    }
}
