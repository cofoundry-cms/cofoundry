using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gives information about the collection that a page block is in.
    /// </summary>
    public class ListablePageBlockRenderContext
    {
        /// <summary>
        /// Total number of blocks in the region this block
        /// is contained in.
        /// </summary>
        public int NumBlocks { get; set; }

        /// <summary>
        /// The zero based position of this block in
        /// the parent collection
        /// </summary>
        public int Index { get; set; }

        #region methods

        /// <summary>
        /// True if this is the first block in the parent collection; otherwise false
        /// </summary>
        public bool IsFirst()
        {
            return Index == 0;
        }

        /// <summary>
        /// True if this is the last block in the parent collection; otherwise false
        /// </summary>
        public bool IsLast()
        {
            return Index == NumBlocks -1;
        }

        #endregion
    }
}
