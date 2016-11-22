using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    /// <summary>
    /// Registers the default locations for page module view files
    /// </summary>
    public class PageModuleLocationRegistration : IPageModuleViewLocationRegistration
    {
        public string[] GetPathPrefixes()
        {
            return new string[] { "PageModules", "Cofoundry/PageModules", "Views/PageModules" };
        }
    }
}