using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core.Web
{
    public class SitePathResolver : IPathResolver
    {
        public string MapPath(string path)
        {
            throw new NotImplementedException();
            //if (!path.StartsWith("~/")) return path;
            //return HostingEnvironment.MapPath(path);
        }
    }
}
