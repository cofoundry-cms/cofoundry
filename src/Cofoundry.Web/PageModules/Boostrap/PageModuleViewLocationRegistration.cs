using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    public class PageModuleLocationRegistration : IPageModuleViewLocationRegistration
    {
        public string[] GetPathPrefixes()
        {
            return new string[] { "PageModules", "Cofoundry/PageModules" };
        }
    }
}