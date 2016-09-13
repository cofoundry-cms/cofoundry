using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class UpdateSeoSettingsCommand : ICommand, ILoggableCommand
    {
        [RegularExpression(@"(UA|YT|MO)-\d+-\d+")]
        [MaxLength(50)]
        public string GoogleAnalyticsUAId { get; set; }

        [MaxLength(160)]
        public string MetaKeywords { get; set; }

        public string RobotsTxt { get; set; }

        public string HumansTxt { get; set; }

        [MaxLength(32)]
        public string BingWebmasterToolsApiKey { get; set; }
    }
}
