using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Validation.Internal;

namespace Cofoundry.Core.Validation.Registration
{
    public class ValidationDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterSingleton<IModelValidationService, ModelValidationService>()
                ;
        }
    }
}
