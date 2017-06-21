using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Registers the default locations for page module view files
    /// </summary>
    public class PageModuleViewLocationRegistration : IPageModuleViewLocationRegistration
    {
        public IEnumerable<string> GetPathPrefixes()
        {
            yield return "/PageModules";
            yield return "/Cofoundry/PageModules";
            yield return "/Views/PageModules";
        }
    }
}