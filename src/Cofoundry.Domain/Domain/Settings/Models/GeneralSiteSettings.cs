using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class GeneralSiteSettings : ICofoundrySettings
    {
        public string ApplicationName { get; set; }
        public bool AllowAutomaticUpdates { get; set; }
    }
}
