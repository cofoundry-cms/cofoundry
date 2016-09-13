using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Web.ModularMvc;

namespace Cofoundry.Web.Admin.Framework.Bootstrap
{
    public class CofoundryAdminViewLocationRegistration : IViewLocationRegistration
    {
        public ViewLocations GetLocations()
        {
            var locations = new ViewLocations()
            {
                ViewLocationFormats = new string[] {
                    RouteConstants.InternalModuleResourcePathPrefix + "{1}/mvc/views/{0}.cshtml"
                },

                PartialViewLocationFormats = new string[] {
                    RouteConstants.InternalModuleResourcePathPrefix + "{1}/mvc/views/partials/{0}.cshtml"
                }
            };

            return locations;
        }
    }
}