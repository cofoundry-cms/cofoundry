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
    /// configuration of applications. Implementations are run in series
    /// without any specific ordering, but all are run after AddMvc has been
    /// invoked.
    /// </summary>
    public interface IStartupServiceConfigurationTask
    {
        /// <summary>
        /// Configures Mvc services. Runs after AddMvc in the service
        /// configuration pipeline.
        /// </summary>
        /// <param name="mvcBuilder">IMvcBuilder to configure.</param>
        void ConfigureServices(IMvcBuilder mvcBuilder);
    }
}