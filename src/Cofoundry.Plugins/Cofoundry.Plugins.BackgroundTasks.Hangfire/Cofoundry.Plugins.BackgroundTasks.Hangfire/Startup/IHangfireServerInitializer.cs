using Microsoft.AspNetCore.Builder;

namespace Cofoundry.Plugins.BackgroundTasks.Hangfire;

/// <summary>
/// Used to initialize the hangfire server. Override this to fully
/// customize the server initialization process.
/// </summary>
public interface IHangfireServerInitializer
{
    void Initialize(IApplicationBuilder app);
}
