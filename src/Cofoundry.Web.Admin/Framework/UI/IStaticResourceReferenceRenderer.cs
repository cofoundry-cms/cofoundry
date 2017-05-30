using Cofoundry.Core;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public interface IStaticResourceReferenceRenderer
    {
        string JsPath(ModuleRouteLibrary moduleRouteLibrary, string fileName);
        string CssPath(ModuleRouteLibrary moduleRouteLibrary, string fileName);
        HtmlString ScriptTag(ModuleRouteLibrary moduleRouteLibrary, string fileName);
        HtmlString ScriptTagIfExists(ModuleRouteLibrary moduleRouteLibrary, string fileName);
        HtmlString CssTag(ModuleRouteLibrary moduleRouteLibrary, string fileName);
        HtmlString CssTagIfExists(ModuleRouteLibrary moduleRouteLibrary, string fileName);
    }
}