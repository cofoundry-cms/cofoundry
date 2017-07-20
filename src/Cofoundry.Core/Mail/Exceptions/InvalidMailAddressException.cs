using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Thrown when an email address cannot be parsed.
    /// </summary>
    public class InvalidMailAddressException : Exception
    {
        const string MESSAGE = "Invalid mail address";

        public InvalidMailAddressException()
            : base(MESSAGE)
        {
        }

        public InvalidMailAddressException(string email, Exception innerEx)
            : base(FormatMessage(email, string.Empty), innerEx)
        {
            MailAddress = email;
        }

        public InvalidMailAddressException(string email, string displayName, Exception innerEx)
            : base(FormatMessage(email, string.Empty), innerEx)
        {
            MailAddress = email;
            DisplayName = displayName;
        }
        
        public string MailAddress { get; private set; }

        public string DisplayName { get; private set; }

        private static string FormatMessage(string email, string displayName)
        {
            return $"Invalid mail address: {email}, display name: {displayName}";
        }
    }
}
