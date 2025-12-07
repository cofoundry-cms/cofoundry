namespace MenuSample;

public class MultiLevelMenuViewModel
{
    public required string MenuId { get; set; }

    public required IReadOnlyCollection<MultiLevelMenuNodeViewModel> Nodes { get; set; }
}
