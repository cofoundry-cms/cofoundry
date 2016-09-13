using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Generic settings for sending mail.
    /// </summary>
    public class MailSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// Indicates whether emails should be sent and how.
        /// </summary>
        public MailSendMode SendMode { get; set; }
        
        /// <summary>
        /// An email address to redirect all mail to when using MailSendMode.SendToDebugAddress
        /// </summary>
        public string DebugEmailAddress { get; set; }

        /// <summary>
        /// The address to send emails from.
        /// </summary>
        [Required]
        public string DefaultFromAddress { get; set; }

        /// <summary>
        /// Optionally the name to display as the From Address
        /// </summary>
        public string DefaultFromAddressDisplayName { get; set; }
    }
}
