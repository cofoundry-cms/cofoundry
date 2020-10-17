using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class CryptographyDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IPasswordCryptographyService, PasswordCryptographyService>()
                .Register<ISecurityTokenGenerationService, SecurityTokenGenerationService>()
                .Register<IPasswordGenerationService, PasswordGenerationService>()
                .Register<IRandomStringGenerator, RandomStringGenerator>()
                ;
        }
    }
}
