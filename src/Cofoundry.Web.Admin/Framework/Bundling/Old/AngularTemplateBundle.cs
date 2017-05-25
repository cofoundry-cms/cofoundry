using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class AngularTemplateBundle : Bundle
    {
        public AngularTemplateBundle(string moduleName, string virtualPath)
            : base(virtualPath, new[] { new AngularTemplateBundleTransform(moduleName) })
        {
        }
    }
}