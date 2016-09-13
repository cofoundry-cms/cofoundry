using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.ErrorLogging
{
    public interface IErrorLoggingService
    {
        void Log(Exception ex);
    }
}
