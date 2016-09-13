using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Configuration;
using Cofoundry.Web.WebApi;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class AppStartDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<IStartupTask>()
                ;
        }
    }
}
