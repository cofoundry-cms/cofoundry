using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Extends the IMvcBuilder configuration to allow for modular configuration
    /// of Mvc services
    /// </summary>
    public class CofoundryStartupServiceConfigurationTask : IStartupServiceConfigurationTask
    {
        private readonly IEnumerable<IMvcJsonOptionsConfiguration> _mvcJsonOptionsConfigurations;
        private readonly IEnumerable<IMvcOptionsConfiguration> _mvcOptionsConfigurations;
        private readonly IEnumerable<IMvcRazorRuntimeCompilationOptionsConfiguration> _razorViewEngineOptionsConfigurations;
        private readonly IAuthConfiguration _authConfiguration;

        public CofoundryStartupServiceConfigurationTask(
            IEnumerable<IMvcJsonOptionsConfiguration> mvcJsonOptionsConfigurations,
            IEnumerable<IMvcOptionsConfiguration> mvcOptionsConfigurations,
            IEnumerable<IMvcRazorRuntimeCompilationOptionsConfiguration> razorViewEngineOptionsConfigurations,
            IAuthConfiguration authConfiguration
            )
        {
            _mvcJsonOptionsConfigurations = mvcJsonOptionsConfigurations;
            _mvcOptionsConfigurations = mvcOptionsConfigurations;
            _razorViewEngineOptionsConfigurations = razorViewEngineOptionsConfigurations;
            _authConfiguration = authConfiguration;
        }

        /// <summary>
        /// Configures Mvc services. Runs after AddMvc in the service
        /// configuration pipeline.
        /// </summary>
        /// <param name="mvcBuilder">IMvcBuilder to configure.</param>
        public void ConfigureServices(IMvcBuilder mvcBuilder)
        {
            // Set MVC compatibility to latest tested version (Not required for 3.1).
            //mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            _authConfiguration.Configure(mvcBuilder);

            foreach (var config in EnumerableHelper
                .Enumerate(_mvcJsonOptionsConfigurations)
                .OrderByDescending(o => o is CofoundryMvcJsonOptionsConfiguration))
            {
                mvcBuilder.Services.Configure<MvcNewtonsoftJsonOptions>(o => config.Configure(o));
            }

            mvcBuilder.AddNewtonsoftJson();

            foreach (var config in EnumerableHelper.Enumerate(_mvcOptionsConfigurations))
            {
                mvcBuilder.Services.Configure<MvcOptions>(o => config.Configure(o));
            }

            foreach (var config in EnumerableHelper
                .Enumerate(_razorViewEngineOptionsConfigurations)
                .OrderByDescending(o => o is CofoundryMvcRazorRuntimeCompilationOptionsConfiguration))
            {
                mvcBuilder.Services.Configure<MvcRazorRuntimeCompilationOptions>(o => config.Configure(o));
            }
        }
    }
}
