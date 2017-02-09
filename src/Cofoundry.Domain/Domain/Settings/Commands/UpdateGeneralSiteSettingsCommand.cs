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
    public class UpdateGeneralSiteSettingsCommand : ICommand, ILoggableCommand
    {
        [Required]
        [MaxLength(100)]
        public string ApplicationName { get; set; }

        public bool AllowAutomaticUpdates { get; set; }
    }
}
