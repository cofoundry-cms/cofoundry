using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// Enum representing the two download modes for
    /// document assets, which map to the two content
    /// disposition values "inline" and "attachement".
    /// </summary>
    public enum DocumentDownloadMode
    {
        /// <summary>
        /// I.e. the default content disposition value "inline"
        /// </summary>
        Inline = 0,

        /// <summary>
        /// I.e. use the content disposition value "attachment"
        /// and force a download dialog.
        /// </summary>
        ForceDownload = 1
    }
}
