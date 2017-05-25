using Cofoundry.Core;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class CustomEntitiesRouteLibrary : AngularModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "custom-entities";

        #endregion

        #region constructor

        public CustomEntitiesRouteLibrary(
            IStaticResourceFileProvider staticResourceFileProvider,
            OptimizationSettings optimizationSettings
            )
            : base(
                  RoutePrefix, 
                  RouteConstants.InternalModuleResourcePathPrefix,
                  staticResourceFileProvider,
                  optimizationSettings
                  )
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