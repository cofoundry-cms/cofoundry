using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cofoundry.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Web
{
    /// <summary>
    /// Configures MVC authentication during the Cofoundry 
    /// startup process. You can override this to completely
    /// customize the authenitcation process.
    /// </summary>
    public interface IAuthConfiguration
    {
        /// <summary>
        /// Applies authentication configuration to the IMvcBuilder.
        /// </summary>
        void Configure(IMvcBuilder mvcBuilder);
    }
}