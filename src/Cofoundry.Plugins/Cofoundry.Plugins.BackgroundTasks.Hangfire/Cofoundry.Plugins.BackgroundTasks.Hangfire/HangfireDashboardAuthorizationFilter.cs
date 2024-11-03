using Cofoundry.Domain;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Plugins.BackgroundTasks.Hangfire;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        var service = context.GetHttpContext().RequestServices.GetRequiredService<IUserContextService>();

        // Hangfire does not support async auth:
        // https://github.com/HangfireIO/Hangfire/issues/827
        var userContext = service
            .GetCurrentContextByUserAreaAsync(CofoundryAdminUserArea.Code)
            .ConfigureAwait(false).GetAwaiter().GetResult();

        return userContext.IsCofoundryUser();
    }
}
