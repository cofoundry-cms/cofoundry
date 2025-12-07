namespace PageBlockTypeSample;

public class PageSnippetDisplayModel : IPageBlockTypeDisplayModel
{
    public PageRenderDetails? Page { get; set; }

    public string Snippet { get; set; } = string.Empty;
}
