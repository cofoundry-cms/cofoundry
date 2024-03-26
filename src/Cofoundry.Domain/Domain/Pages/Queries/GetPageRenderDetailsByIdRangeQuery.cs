namespace Cofoundry.Domain;

/// <summary>
/// Gets a range of pages by their ids projected as PageRenderDetails models. A PageRenderDetails contains 
/// the data required to render a page, including template data for all the content-editable regions.
/// </summary>
public class GetPageRenderDetailsByIdRangeQuery : IQuery<IReadOnlyDictionary<int, PageRenderDetails>>
{
    public GetPageRenderDetailsByIdRangeQuery()
    {
        PageIds = new List<int>();
    }

    /// <summary>
    /// Gets a range of pages by their ids projected as PageRenderDetails models. A PageRenderDetails contains 
    /// the data required to render a page, including template data for all the content-editable regions.
    /// </summary>
    /// <param name="pageIds">PageIds of the pages to get.</param>
    /// <param name="publishStatusQuery">Used to determine which version of the page to include data for.</param>
    public GetPageRenderDetailsByIdRangeQuery(IEnumerable<int> pageIds, PublishStatusQuery? publishStatusQuery = null)
        : this(pageIds?.ToArray() ?? [], publishStatusQuery)
    {
    }

    /// <summary>
    /// Gets a range of pages by their ids projected as PageRenderDetails models. A PageRenderDetails contains 
    /// the data required to render a page, including template data for all the content-editable regions.
    /// </summary>
    /// <param name="pageIds">PageIds of the pages to get.</param>
    /// <param name="publishStatusQuery">Used to determine which version of the page to include data for.</param>
    public GetPageRenderDetailsByIdRangeQuery(IReadOnlyCollection<int> pageIds, PublishStatusQuery? publishStatusQuery = null)
    {
        PageIds = pageIds;
        if (publishStatusQuery.HasValue)
        {
            PublishStatus = publishStatusQuery.Value;
        }
    }

    /// <summary>
    /// Database ids of the pages to get.
    /// </summary>
    public IReadOnlyCollection<int> PageIds { get; set; }

    /// <summary>
    /// Used to determine which version of the page to include data for. This 
    /// defaults to Published, meaning that only published pages will be returned.
    /// </summary>
    public PublishStatusQuery PublishStatus { get; set; }
}
