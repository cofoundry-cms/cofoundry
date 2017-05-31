using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Web
{
    public static class IContainerRegisterExtensions
    {
        public static IContainerRegister RegisterDatabase<T>(this IContainerRegister container) where T : DbContext
        {
            var options = new RegistrationOptions();
            options.InstanceScope = InstanceScope.PerLifetimeScope;
            return container.RegisterType<T, T>(options);
        }
    }
}
