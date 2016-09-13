using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class ModuleStaticContentRouteLibrary
    {
        #region constructor

        private readonly ModuleRouteLibrary _moduleRouteLibrary;

        public ModuleStaticContentRouteLibrary(
            ModuleRouteLibrary moduleRouteLibrary
            )
        {
            _moduleRouteLibrary = moduleRouteLibrary;
            EmbeddedResourceRegistrationPath = _moduleRouteLibrary.ResourcePrefix + "content";
        }

        #endregion

        /// <summary>
        /// Route formatted for registering js files as embedded resources.
        /// </summary>
        public string EmbeddedResourceRegistrationPath { get; private set; }

        /// <summary>
        /// Creates a url for the specified static resource file.
        /// </summary>
        /// <param name="fileName">
        /// Filename for the resource, which can include path information 
        /// if it is in a subdirectory of the content folder.
        /// </param>
        public string Url(string fileName)
        {
            return EmbeddedResourceRegistrationPath + "/" + fileName;
        }
    }
}