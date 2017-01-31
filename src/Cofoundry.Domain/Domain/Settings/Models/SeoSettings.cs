using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class SeoSettings : ICofoundrySettings
    {
        public string GoogleAnalyticsUAId { get; set; }

        public string RobotsTxt { get; set; }

        public string HumansTxt { get; set; }

        public string BingWebmasterToolsApiKey { get; set; }
    }
}
