using Cofoundry.Core;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class CustomEntitiesRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "custom-entities";

        public static readonly CustomEntitiesRouteLibrary Urls = new CustomEntitiesRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public CustomEntitiesRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region routes

        public string List(CustomEntityDefinitionSummary summary)
        {
            if (summary == null) return string.Empty;
            return "/" + RouteConstants.AdminAreaPrefix + "/" + SlugFormatter.ToSlug(summary.NamePlural) + "#/";
        }

        public string New(CustomEntityDefinitionSummary summary)
        {
            if (summary == null) return string.Empty;
            return List(summary) + "new";
        }

        public string Details(CustomEntityDefinitionSummary summary, int id)
        {
            if (summary == null) return string.Empty;
            return List(summary) + id.ToString();
        }
        
        #endregion
    }
}