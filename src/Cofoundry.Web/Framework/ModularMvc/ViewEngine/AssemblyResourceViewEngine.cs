using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Custom view engine that allows views to be pulled from assembly resources
    /// and removes support for ascx/vbhtml pages to speed things up.
    /// </summary>
    public class AssemblyResourceViewEngine : RazorViewEngine
    {
        public AssemblyResourceViewEngine(
            IViewLocationRegistration[] viewLocationRegistrations
            )
        {
            // Make sure we put the default view locations up front so these are checked first.
            var locations = viewLocationRegistrations
                .OrderByDescending(l => l is DefaultViewLocationRegistration)
                .Select(l => l.GetLocations())
                .ToList();

            Init(locations);
        }

        private void Init(IEnumerable<ViewLocations> locations)
        {
            AreaMasterLocationFormats = GetDistinct(locations.Select(l => l.AreaMasterLocationFormats));
            AreaViewLocationFormats = GetDistinct(locations.Select(l => l.AreaViewLocationFormats));
            AreaPartialViewLocationFormats = GetDistinct(locations.Select(l => l.AreaPartialViewLocationFormats));

            MasterLocationFormats = GetDistinct(locations.Select(l => l.MasterLocationFormats));
            ViewLocationFormats = GetDistinct(locations.Select(l => l.ViewLocationFormats));
            PartialViewLocationFormats = GetDistinct(locations.Select(l => l.PartialViewLocationFormats));
        }

        private string[] GetDistinct(IEnumerable<string[]> arr)
        {
            return arr
                .Where(l => l != null)
                .SelectMany(l => l)
                .Distinct()
                .ToArray();
        }
    }
}