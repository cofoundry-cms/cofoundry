using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Flagged enum providing a variety of format options when converting
    /// a string to Html
    /// </summary>
    [Flags]
    public enum BasicHtmlFormatOption
    {
        None = 0,

        /// <summary>
        /// Make links open in a new window
        /// </summary>
        LinksToNewWindow = 1,

        /// <summary>
        /// Mark links as no-follow
        /// </summary>
        LinksNoFollow = 2
    }

}
