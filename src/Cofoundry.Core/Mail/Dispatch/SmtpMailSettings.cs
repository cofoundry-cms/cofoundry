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
        public SmtpMailSettings()
        {
            DebugMailDropDirectory = "~/App_Data/Emails";
        }
        /// <summary>
        /// The path to the folder to save mail to when using SendMode.LocalDrop. Defaults 
        /// to ~/App_Data/Emails
        /// </summary>
        public string DebugMailDropDirectory { get; set; }
    }
}
