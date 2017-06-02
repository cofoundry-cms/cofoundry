using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public interface IAngularBootstrapper
    {
        /// <summary>
        /// Adds scripts/templates for the core angular framework and the
        /// specified module and then bootstraps it.
        /// </summary>
        /// <param name="routeLibrary">Js routing library for the module to bootstrap,</param>
        Task<IHtmlContent> BootstrapAsync(AngularModuleRouteLibrary routeLibrary, object options = null);
    }
}