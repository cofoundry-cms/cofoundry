using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gives information about the collection that a module is in.
    /// </summary>
    public class ListablePageModuleRenderContext
    {
        /// <summary>
        /// Total number of modules in the section this module
        /// is contained in.
        /// </summary>
        public int NumModules { get; set; }

        /// <summary>
        /// The zero based position of this module in
        /// the parent collection
        /// </summary>
        public int Index { get; set; }

        #region methods

        /// <summary>
        /// True if this is the first module in the parent collection; otherwise false
        /// </summary>
        public bool IsFirst()
        {
            return Index == 0;
        }

        /// <summary>
        /// True if this is the last module in the parent collection; otherwise false
        /// </summary>
        public bool IsLast()
        {
            return Index == NumModules -1;
        }

        #endregion
    }
}
