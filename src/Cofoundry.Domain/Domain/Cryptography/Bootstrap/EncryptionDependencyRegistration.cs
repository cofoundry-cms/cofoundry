using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain
{
    public class EncryptionDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IPasswordCryptographyService, PasswordCryptographyService>()
                .Register<ISecurityTokenGenerationService, SecurityTokenGenerationService>()
                .Register<IPasswordGenerationService, PasswordGenerationService>();
        }
    }
}
