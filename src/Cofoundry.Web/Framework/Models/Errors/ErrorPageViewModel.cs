namespace Cofoundry.Web;

public class ErrorPageViewModel : IErrorPageViewModel
{
    public int StatusCode { get; set; }

    public string StatusCodeDescription { get; set; } = string.Empty;

    public string PageTitle { get; set; } = string.Empty;

    public string? MetaDescription { get; set; }

    public string PathBase { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string? QueryString { get; set; }
}
