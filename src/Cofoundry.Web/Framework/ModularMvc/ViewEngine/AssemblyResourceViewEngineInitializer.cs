using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Deals with initialization of the AssemblyResourceViewEngine
    /// by registering it as the only asp.net mvc ViewEngine
    /// </summary>
    public class AssemblyResourceViewEngineInitializer : IAssemblyResourceViewEngineInitializer
    {
        private readonly VirtualPathProvider _vitualPathProvider;
        private readonly RazorViewEngine _viewEngine;

        public AssemblyResourceViewEngineInitializer(
            VirtualPathProvider virtualPathProvider,
            RazorViewEngine viewEngine
            )
        {
            _vitualPathProvider = virtualPathProvider;
            _viewEngine = viewEngine;
        }

        /// <summary>
        /// Registers the AssemblyResourceViewEngine
        /// </summary>
        public void Initialize()
        {
            // Configure the assembly resource provider that gets all the views out of the DLL
            HostingEnvironment.RegisterVirtualPathProvider(_vitualPathProvider);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(_viewEngine);
        }
    }
}
