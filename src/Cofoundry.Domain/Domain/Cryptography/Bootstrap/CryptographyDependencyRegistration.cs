using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Identity;

namespace Cofoundry.Domain.Registration
{
    public class CryptographyDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IPasswordCryptographyService, PasswordCryptographyService>()
                .Register<IPasswordGenerationService, PasswordGenerationService>()
                .Register<IRandomStringGenerator, RandomStringGenerator>()
                .Register<IPasswordHasher<PasswordHasherUser>, PasswordHasher<PasswordHasherUser>>()
                ;
        }
    }
}
