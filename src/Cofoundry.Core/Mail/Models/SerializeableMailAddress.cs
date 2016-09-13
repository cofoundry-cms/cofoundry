using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Serializable version of System.Net.Mail.MailAddress
    /// </summary>
    [Serializable]
    public class SerializeableMailAddress
    {
        #region constructors

        public SerializeableMailAddress()
        {
        }

        public SerializeableMailAddress(string address)
            : this(new MailAddress(address))
        {
        }

        public SerializeableMailAddress(string address, string displayName)
            : this(new MailAddress(address, displayName))
        {
        }

        public SerializeableMailAddress(MailAddress mailAddress)
        {
            Address = mailAddress.Address;
            DisplayName = mailAddress.DisplayName;
        }


        #endregion

        /// <summary>
        /// E-mail address e.g. 'john.smith@gmail.com'
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// User friendly display name e.g. 'John Smith'
        /// </summary>
        public string DisplayName { get; set; }

        public MailAddress ToMailAddress()
        {
            return new MailAddress(Address, DisplayName);
        }

        public override string ToString()
        {
            return Address;
        }
    }
}
