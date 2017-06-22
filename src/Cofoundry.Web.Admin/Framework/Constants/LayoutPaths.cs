using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Path constants to the shared layout files used in the admin site.
    /// </summary>
    public static class LayoutPaths
    {
        private const string LAYOUT_BASE = "~/Admin/Modules/Shared/MVC/Views/Layouts/";

        /// <summary>
        /// The base layout shared amoung other layout files, not to be used directly.
        /// </summary>
        public static readonly string BaseLayout = LAYOUT_BASE + "_BaseLayout.cshtml";

        /// <summary>
        /// A bare layout without the admin left menu
        /// </summary>
        public static readonly string SharedLayout = LAYOUT_BASE + "_SharedLayout.cshtml";

        /// <summary>
        /// The default admin layout to use with views. this contains the left admin menu.
        /// </summary>
        public static readonly string DefaultLayout = LAYOUT_BASE + "_DefaultLayout.cshtml";

        /// <summary>
        /// A minimal layout consisting of just the admin frame, used for error messages and the
        /// site Setup page.
        /// </summary>
        public static readonly string MinimalLayout = LAYOUT_BASE + "_MinimalLayout.cshtml";
    }
}