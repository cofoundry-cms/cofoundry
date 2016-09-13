using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Web.ModularMvc;

namespace Cofoundry.Web.Admin
{
    public class SharedModuleViewLocationRegistration : IViewLocationRegistration
    {
        public ViewLocations GetLocations()
        {
            var locations = new ViewLocations()
            {
                AreaViewLocationFormats = new string[] {
                    RouteConstants.InternalModuleResourcePathPrefix + "shared/mvc/views/{0}.cshtml"
                },

                PartialViewLocationFormats = new string[] {
                    RouteConstants.InternalModuleResourcePathPrefix + "shared/mvc/views/partials/{0}.cshtml"
                },

                MasterLocationFormats = new string[] {
                    RouteConstants.InternalModuleResourcePathPrefix + "shared/mvc/views/{0}.cshtml"
                }
            };

            return locations;
        }
    }
}