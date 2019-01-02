using System;
using System.Net;
using Cofoundry.Core.AutoUpdate;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Cofoundry.Core;
using Microsoft.AspNetCore.Hosting;

namespace Cofoundry.Web
{
    /// <summary>
    /// Checks to see if the auto-update is running and reacts accordingly. If the updater
    /// has completed then the next middleware in the pipline is executed unimpeded. If 
    /// the updater is running or locked then the middleware will wait a short time to see if it 
    /// completes.
    /// </summary>
    public class AutoUpdateMiddleware
    {
        private readonly RequestDelegate _next;

        public AutoUpdateMiddleware(
            RequestDelegate next
            ) 
        {
            _next = next;
        }

        public async Task Invoke(HttpContext cx)
        {
            // Ignore the re-executing error handler in case it originated from this middleware
            if (IsHandlingError(cx))
            {
                await _next.Invoke(cx);
                return;
            }

            var autoUpdateState = cx.RequestServices.GetService<AutoUpdateState>();
            var state = autoUpdateState.Status;

            switch (state)
            {
                case AutoUpdateStatus.Complete:
                    // Execute the next middleware in the pipline immediately
                    await _next.Invoke(cx);
                    break;

                case AutoUpdateStatus.Error:
                    // Throw an error which will redirect the user to the error handling page. 
                    throw new AutoUpdateFailedException(autoUpdateState.Exception);
                case AutoUpdateStatus.LockedByAnotherProcess:
                case AutoUpdateStatus.NotStarted:
                case AutoUpdateStatus.InProgress:
                    // If the auto-updater is running or has not run yet, then wait a while to
                    // see if it completes before returning a 503 (service unavilable) response.
                    await WaitForCompletionAsync(cx, autoUpdateState);
                    break;

                default:
                    throw new Exception("AutoUpdateStatus not recognized");
            }
        }

        private async Task WaitForCompletionAsync(
            HttpContext cx, 
            AutoUpdateState autoUpdateState
            )
        {
            var updatedStatus = autoUpdateState.Status;
            var autoUpdateSettings = cx.RequestServices.GetService<AutoUpdateSettings>();
            var waitTimeLimit = TimeSpan.FromSeconds(autoUpdateSettings.RequestWaitForCompletionTimeInSeconds);
            var waitInterval = TimeSpan.FromSeconds(0.3);

            double totalWaitTimeInMilliseconds = 0;
            while (totalWaitTimeInMilliseconds <= waitTimeLimit.TotalMilliseconds && updatedStatus != AutoUpdateStatus.Complete)
            {
                await Task.Delay(waitInterval);
                updatedStatus = autoUpdateState.Status;

                totalWaitTimeInMilliseconds += waitInterval.TotalMilliseconds;
            }

            if (updatedStatus == AutoUpdateStatus.Complete)
            {
                await _next.Invoke(cx);
            }
            else if (updatedStatus == AutoUpdateStatus.Error)
            {
                // Throw an error which will redirect the user to the error handling page. 
                throw new AutoUpdateFailedException(autoUpdateState.Exception);
            }
            else
            {
                string msg;
                if (updatedStatus == AutoUpdateStatus.LockedByAnotherProcess)
                {
                    msg = "The site is being updated and is currently locked. Please try again shortly.";
                }
                else
                {
                    msg = "The site is being updated. Please try again shortly.";
                }

                await WriteUnavailableResponse(cx, msg);
            }
        }

        private static bool IsHandlingError(HttpContext cx)
        {
            return cx.Features.Get<IExceptionHandlerPathFeature>() != null 
                || cx.Features.Get<IStatusCodeReExecuteFeature>() != null;
        }

        private static Task WriteUnavailableResponse(HttpContext cx, string msg)
        {
            cx.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            return cx.Response.WriteAsync(msg);
        }
    }
}