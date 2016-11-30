using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.ModularMvc
{
    public class DefaultViewLocationRegistration : IViewLocationRegistration
    {
        public ViewLocations GetLocations()
        {
            var locations = new ViewLocations()
            {
                AreaViewLocationFormats = new string[] {
                    "~/Areas/{2}/Views/{1}/{0}.cshtml",
                    "~/Areas/{2}/Views/Shared/{0}.cshtml"
                },

                AreaMasterLocationFormats = new string[] {
                    "~/Areas/{2}/Views/{1}/{0}.cshtml",
                    "~/Areas/{2}/Views/Shared/{0}.cshtml"
                },

                AreaPartialViewLocationFormats = new string[] {
                    "~/Areas/{2}/Views/{1}/{0}.cshtml",
                    "~/Areas/{2}/Views/{1}/Partials/{0}.cshtml",
                    "~/Areas/{2}/Views/Shared/{0}.cshtml",
                    "~/Areas/{2}/Views/Shared/Partials/{0}.cshtml"
                },

                ViewLocationFormats = new string[] {
                    "~/Views/{1}/{0}.cshtml",
                    "~/Views/Shared/{0}.cshtml",
                    "~/Cofoundry/PageTemplates/{0}.cshtml"
                },

                PartialViewLocationFormats = new string[] {
                    "~/Views/{1}/Partials/{0}.cshtml",
                    "~/Views/{1}/{0}.cshtml",
                    "~/Views/Shared/{0}.cshtml",
                    "~/Views/Shared/Partials/{0}.cshtml",
                    "~/Cofoundry/PageTemplates/Partials/{0}.cshtml"
                },

                MasterLocationFormats = new string[] {
                    "~/Views/{1}/{0}.cshtml",
                    "~/Views/Shared/{0}.cshtml"
                },
            };

            return locations;
        }
    }
}
