using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A primary ordering that is used to partition
    /// menu item ordering in a meaningful way before a more ambiguous
    /// secondary ordering is used.
    /// </summary>
    public enum AdminModuleMenuPrimaryOrdering
    {
        /// <summary>
        /// Reserved for essentials core modules that should 
        /// always appear at the top like Pages
        /// </summary>
        Primary = 0,

        /// <summary>
        /// Other core modules that are less important like Images and Documents
        /// </summary>
        Secondry = 10,

        /// <summary>
        /// Custom apps that are of less important than core site functionality.
        /// </summary>
        Tertiary = 20,

        /// <summary>
        /// Anything that shoyuld be appear right at the bottom and is seldom used
        /// </summary>
        Last = 30
    }
}
