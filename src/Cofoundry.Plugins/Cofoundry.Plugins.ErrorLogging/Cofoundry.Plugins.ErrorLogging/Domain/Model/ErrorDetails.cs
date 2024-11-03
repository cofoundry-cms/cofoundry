namespace Cofoundry.Plugins.ErrorLogging.Domain;

public class ErrorDetails
{
    public int ErrorId { get; set; }
    public string ExceptionType { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public string? Session { get; set; }
    public string? Form { get; set; }
    public string? Data { get; set; }
    public string? UserAgent { get; set; }
    public bool EmailSent { get; set; }
    public DateTime CreateDate { get; set; }
}
