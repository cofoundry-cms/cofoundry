namespace MenuSample;

public class NestedMenuItemViewModel
{
    public required string Title { get; set; }

    public required PageRoute PageRoute { get; set; }

    public required IReadOnlyCollection<NestedMenuChildItemViewModel> ChildItems { get; set; }
}
