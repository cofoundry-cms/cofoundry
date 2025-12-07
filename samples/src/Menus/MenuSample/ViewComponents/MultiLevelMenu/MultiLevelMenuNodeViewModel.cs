namespace MenuSample;

public class MultiLevelMenuNodeViewModel
{
    public required string Title { get; set; }

    public required PageRoute PageRoute { get; set; }

    public required IReadOnlyCollection<MultiLevelMenuNodeViewModel> ChildNodes { get; set; }
}
