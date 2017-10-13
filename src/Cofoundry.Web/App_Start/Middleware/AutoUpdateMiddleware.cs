using System;
using System.Net;
using Cofoundry.Core.AutoUpdate;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cofoundry.Web
{
    /// <summary>
    /// Runs the auto-updater module during the first request and locks out other requests
    /// until the update is complete.
    /// </summary>
    public class AutoUpdateMiddleware
    {
        private enum UpdateStatus
        {
            NotStarted,
            InProgress,
            Complete
        }

        private static UpdateStatus _updateStatus = UpdateStatus.NotStarted;
        private static object _updateStatusLock = new object();

        private readonly RequestDelegate _next;

        public AutoUpdateMiddleware(
            RequestDelegate next
            ) 
        {
            _next = next;
        }

        public async Task Invoke(HttpContext cx, IAutoUpdateService autoUpdateService)
        {
            bool runUpdate = false;

            if (_updateStatus == UpdateStatus.NotStarted)
            {
                lock (_updateStatusLock)
                {
                    if (_updateStatus == UpdateStatus.NotStarted)
                    {
                        _updateStatus = UpdateStatus.InProgress;
                        runUpdate = true;
                    }
                }

                if (runUpdate)
                {
                    try
                    {
                        await autoUpdateService.UpdateAsync();
                        _updateStatus = UpdateStatus.Complete;
                    }
                    catch (Exception ex)
                    {
                        _updateStatus = UpdateStatus.NotStarted;
                        throw;
                    }
                }
            }

            if (_updateStatus == UpdateStatus.InProgress)
            {
                cx.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await cx.Response.WriteAsync("The application is being updated. Please try again shortly.");
            }
            else
            {
                await _next.Invoke(cx);
            }
        }
    }
}