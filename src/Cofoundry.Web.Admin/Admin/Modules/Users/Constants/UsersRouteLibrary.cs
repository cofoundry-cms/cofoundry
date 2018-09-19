using Cofoundry.Core;
using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class UsersRouteLibrary : AngularModuleRouteLibrary
    {
        public const string RoutePrefix = "users";
        private readonly AdminSettings _adminSettings;

        public UsersRouteLibrary(AdminSettings adminSettings)
            : base(adminSettings, RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
            _adminSettings = adminSettings;
        }

        #region routes

        public string List(IUserAreaDefinition definition)
        {
            return GetUserAreaRoute(definition);
        }

        public string New(IUserAreaDefinition definition)
        {
            if (definition == null) return string.Empty;
            return List(definition) + "new";
        }

        public string Details(IUserAreaDefinition definition, int id)
        {
            if (definition == null) return string.Empty;
            return List(definition) + id.ToString();
        }

        private string GetUserAreaRoute(IUserAreaDefinition definition, string route = null)
        {
            if (definition == null) return string.Empty;
            return "/" + _adminSettings.DirectoryName + "/" + SlugFormatter.ToSlug(definition.Name) + "-users#/" + route;
        }

        #endregion
    }
}