using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.ErrorLogging
{
    public class ErrorLoggingDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<IErrorLoggingService, SimpleErrorLoggingService>()
                ;
        }
    }
}
