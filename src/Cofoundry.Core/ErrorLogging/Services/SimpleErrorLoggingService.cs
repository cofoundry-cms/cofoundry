using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SimpleErrorLoggingService> _logger;

        public SimpleErrorLoggingService(
            ILogger<SimpleErrorLoggingService> logger
            )
        {
            _logger = logger;
        }

        public void Log(Exception ex)
        {
            _logger.LogError(0, ex, ex.Message);
        }
    }
}
