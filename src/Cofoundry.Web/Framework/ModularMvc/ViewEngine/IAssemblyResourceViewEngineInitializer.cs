using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Deals with initialization of the AssemblyResourceViewEngine
    /// by registering it as the only asp.net mvc ViewEngine
    /// </summary>
    public interface IAssemblyResourceViewEngineInitializer
    {
        /// <summary>
        /// Registers the AssemblyResourceViewEngine
        /// </summary>
        void Initialize();
    }
}
