namespace PageBlockTypeSample;

public class PageListDisplayModel : IPageBlockTypeDisplayModel
{
    public required IReadOnlyCollection<PageRoute> Pages { get; set; }
}
