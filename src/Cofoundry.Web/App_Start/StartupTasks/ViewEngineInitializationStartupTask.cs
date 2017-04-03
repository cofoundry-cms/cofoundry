using Cofoundry.Web.ModularMvc;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Initialized the custom view engine required to pull in embedded resources from
    /// Cofoundry modules.
    /// </summary>
    public class ViewEngineInitializationStartupTask : IStartupTask
    {
        private readonly IAssemblyResourceViewEngineInitializer _assemblyResourceViewEngineInitializer;

        public ViewEngineInitializationStartupTask(
            IAssemblyResourceViewEngineInitializer assemblyResourceViewEngineInitializer
            )
        {
            _assemblyResourceViewEngineInitializer = assemblyResourceViewEngineInitializer;
        }

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Early; }
        }

        public void Run(IApplicationBuilder app)
        {
            _assemblyResourceViewEngineInitializer.Initialize();
        }
    }
}