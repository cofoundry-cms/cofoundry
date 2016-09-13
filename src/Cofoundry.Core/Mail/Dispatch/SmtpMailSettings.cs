using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Core.Mail
{
    public class SmtpMailSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// The path to the folder to save mail to when using SendMode.LocalDrop.
        /// </summary>
        public string DebugMailDropDirectory { get; set; }
    }
}
