using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Cofoundry.Core.AutoUpdate;
using Microsoft.Owin;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Runs the auto-updater module during the first request and locks out other requests
    /// until the update is complete.
    /// </summary>
    public class AutoUpdateMiddleware : OwinMiddleware
    {
        private enum UpdateStatus
        {
            NotStarted,
            InProgress,
            Complete
        }

        private static UpdateStatus _updateStatus = UpdateStatus.NotStarted;
        private static object _updateStatusLock = new object();

        public AutoUpdateMiddleware(OwinMiddleware next) 
            : base(next)
        {
        }

        public async override Task Invoke(IOwinContext cx)
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
                        using (var cs = IckyDependencyResolution.CreateNewChildContextFromRoot())
                        {
                            var service = cs.Resolve<IAutoUpdateService>();
                            await service.UpdateAsync();
                        }
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
                await Next.Invoke(cx);
            }
        }
    }
}