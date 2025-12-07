namespace MenuSample;

public class NestedMenuViewModel
{
    public required string MenuId { get; set; }

    public required IReadOnlyCollection<NestedMenuItemViewModel> Items { get; set; }
}
