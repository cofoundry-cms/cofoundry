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
        public IEnumerable<string> GetPathPrefixes()
        {
            yield return "PageModules";
            yield return "Cofoundry/PageModules";
            yield return "Views/PageModules";
        }
    }
}