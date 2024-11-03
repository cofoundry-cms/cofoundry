namespace Cofoundry.Plugins.ErrorLogging.Domain;

public interface IErrorLoggingService
{
    Task LogAsync(Exception ex);
}
