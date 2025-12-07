namespace PageBlockTypeSample;

public class DirectoryListDisplayModel : IPageBlockTypeDisplayModel
{
    public required IPagedQueryResult<PageRenderSummary> Pages { get; set; }
}
