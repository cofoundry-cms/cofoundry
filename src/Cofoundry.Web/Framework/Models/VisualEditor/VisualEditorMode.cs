using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// The view modes of a page when logged into
    /// the admin panel.
    /// </summary>
    public enum VisualEditorMode
    {
        /// <summary>
        /// Shows the latest version of the page, showing a draft if it is available
        /// and falling back to the published version.
        /// </summary>
        Any,

        /// <summary>
        /// Shows the page in it's published stats, i.e. as it appears to users
        /// who are not logged into the admin panel.
        /// </summary>
        Live,

        /// <summary>
        /// Shows the page in it's draft state (read only)
        /// </summary>
        Draft,

        /// <summary>
        /// Shows the page in it's draft state and additionally allows
        /// editing of the site content.
        /// </summary>
        Edit,

        /// <summary>
        /// Shows a previously published version of a page.
        /// </summary>
        SpecificVersion,

    }
}
