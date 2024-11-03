using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Plugins.BackgroundTasks.Hangfire;

public class HangfireInitializationMiddleware
{
    private static bool _isInitialized;
    private static readonly object _isInitializedLock = new();
    private readonly RequestDelegate _next;

    public HangfireInitializationMiddleware(
        RequestDelegate next
        )
    {
        _next = next;
    }

    public async Task Invoke(HttpContext cx)
    {
        var runInitialize = false;

        if (!_isInitialized)
        {
            lock (_isInitializedLock)
            {
                if (!_isInitialized)
                {
                    _isInitialized = true;
                    runInitialize = true;
                }
            }

            if (runInitialize)
            {
                try
                {
                    var initializer = cx.RequestServices.GetRequiredService<IHangfireBackgroundTaskInitializer>();
                    initializer.Initialize();
                }
                catch (Exception)
                {
                    _isInitialized = false;
                    throw;
                }
            }
        }

        await _next.Invoke(cx);
    }
}
