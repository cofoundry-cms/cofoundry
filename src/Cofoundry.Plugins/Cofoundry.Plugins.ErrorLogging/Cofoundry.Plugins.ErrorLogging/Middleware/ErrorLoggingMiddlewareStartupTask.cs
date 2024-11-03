using Cofoundry.Web;
using Microsoft.AspNetCore.Builder;

namespace Cofoundry.Plugins.ErrorLogging;

public class ErrorLoggingMiddlewareStartupTask : IRunAfterStartupConfigurationTask
{
    public int Ordering
    {
        get { return (int)StartupTaskOrdering.First; }
    }

    public IReadOnlyCollection<Type> RunAfter => [typeof(ErrorHandlingMiddlewareConfigurationTask)];

    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorLoggingMiddleware>();
    }
}
