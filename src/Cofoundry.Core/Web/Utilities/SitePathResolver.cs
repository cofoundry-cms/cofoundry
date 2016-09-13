using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Cofoundry.Core.Web
{
    public class SitePathResolver : IPathResolver
    {
        public string MapPath(string path)
        {
            if (!path.StartsWith("~/")) return path;
            return HostingEnvironment.MapPath(path);
        }
    }
}
