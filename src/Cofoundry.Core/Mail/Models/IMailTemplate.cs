using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Represents an email template including the path to a template file and
    /// any data properties required by the view.
    /// </summary>
    public interface IMailTemplate
    {
        /// <summary>
        /// Name or full path to the view file. This should not include the type part 
        /// or file extension (i.e. '_Html.cshml' or '_Text.cshml') because this is automatically added
        /// </summary>
        string ViewFile { get; }

        /// <summary>
        /// String to use as the subject to the email
        /// </summary>
        string Subject { get; }
    }
}
