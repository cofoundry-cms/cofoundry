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

        public CustomEntitiesRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region routes

        public string List(CustomEntityDefinitionSummary definition)
        {
            return GetCustomEntityRoute(definition?.NamePlural);
        }

        public string List(ICustomEntityDefinition definition)
        {
            return GetCustomEntityRoute(definition?.NamePlural);
        }

        public string New(CustomEntityDefinitionSummary definition)
        {
            if (definition == null) return string.Empty;
            return List(definition) + "new";
        }

        public string Details(CustomEntityDefinitionSummary definition, int id)
        {
            if (definition == null) return string.Empty;
            return List(definition) + id.ToString();
        }

        private static string GetCustomEntityRoute(string namePlural, string route = null)
        {
            if (namePlural == null) return string.Empty;
            return "/" + RouteConstants.AdminAreaPrefix + "/" + SlugFormatter.ToSlug(namePlural) + "#/" + route;
        }

        #endregion
    }
}