using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.ErrorLogging
{
    /// <summary>
    /// The default implementation is really just a placeholder for one of
    /// the plugin implementations.
    /// </summary>
    public class SimpleErrorLoggingService : IErrorLoggingService
    {
        public void Log(Exception ex)
        {
            Trace.TraceError(ex.Message);
        }
    }
}
