using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Extends the IMvcBuilder configuration to allow for modular
    /// configuration of applications. It's best not to override this
    /// unless you really need to, rather implement one of the single-purpose
    /// configuration extensibility interfaces such as IMvcOptionsConfiguration,
    /// IMvcJsonOptionsConfiguration, or IRazorViewEngineOptionsConfiguration
    /// </summary>
    public interface IMvcBuilderConfiguration
    {
        /// <summary>
        /// Configures Mvc services. Runs after AddMvc in the service
        /// configuration pipeline.
        /// </summary>
        /// <param name="mvcBuilder">IMvcBuilder to configure.</param>
        void Configure(IMvcBuilder mvcBuilder);
    }
}