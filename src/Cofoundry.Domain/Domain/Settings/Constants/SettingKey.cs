using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Constants db identifiers for values in the
    /// settings table.
    /// </summary>
    public static class SettingKey
    {
        public const string ApplicationName = "ApplicationName";
        public const string GoogleAnalyticsUAId = "GoogleAnalyticsUAId";
        public const string HumansTxt = "HumansTxt";
        public const string RobotsTxt = "RobotsTxt";
    }
}
