using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Cofoundry.Core.ErrorLogging;

namespace Cofoundry.Web.WebApi
{
    public class WebApiExceptionLogger : ExceptionLogger 
    {
        public override void Log(ExceptionLoggerContext context)
        {
            using (var resolutionContext = IckyDependencyResolution.CreateNewChildContextFromRoot())
            {
                var errorLoggingService = resolutionContext.Resolve<IErrorLoggingService>();
                errorLoggingService.Log(context.Exception);
            }

            base.Log(context);
        }
    }
}
