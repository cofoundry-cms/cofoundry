using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class AdminModule
    {
        /// <remarks>
        /// Uniquely identifies the module
        /// </remarks>
        public string AdminModuleCode { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public AdminModuleMenuCategory MenuCategory { get; set; }

        /// <summary>
        /// A primary ordering that is used to partition
        /// menu item ordering in a meaningful way before a more ambiguous
        /// secondary ordering is used.
        /// </summary>
        public AdminModuleMenuPrimaryOrdering PrimaryOrdering { get; set; }

        /// <summary>
        /// The ordering that is applied after primary ordering when constructing the admin
        /// menu in the GUI.
        /// </summary>
        public int SecondaryOrdering { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// The IPermission to check to see if the user is allowed to 
        /// view this module.
        /// </summary>
        public IPermission RestrictedToPermission { get; set; }

        public string GetMenuLinkByPath(string path)
        {
            if (Url != null && Url.StartsWith(path, StringComparison.OrdinalIgnoreCase))
            {
                return Url;
            }

            return null;
        }
    }
}
