using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.ModularMvc
{
    public class ViewLocations
    {
        public string[] AreaViewLocationFormats { get; set; }
        public string[] AreaMasterLocationFormats { get; set; }
        public string[] AreaPartialViewLocationFormats { get; set; }

        public string[] ViewLocationFormats { get; set; }
        public string[] PartialViewLocationFormats { get; set; }
        public string[] MasterLocationFormats { get; set; }
    }
}
