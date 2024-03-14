namespace Cofoundry.Web;

public class DocumentDisplayModel : IPageBlockTypeDisplayModel
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
