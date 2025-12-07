namespace Cofoundry.Web;

public class TextListDisplayModel : IPageBlockTypeDisplayModel
{
    public required string? Title { get; set; }
    public required IReadOnlyCollection<string> TextListItems { get; set; }
    public required bool IsNumbered { get; set; }
}
