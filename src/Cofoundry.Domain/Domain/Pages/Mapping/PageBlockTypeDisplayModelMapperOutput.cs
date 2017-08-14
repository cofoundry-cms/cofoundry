using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A wrapper object containing a page block display model
    /// and an identifier that is used to put it back into 
    /// a collection after mapping.
    /// </summary>
    public class PageBlockTypeDisplayModelMapperOutput
    {
        /// <summary>
        /// The id of the versioned entity to which this block belongs
        /// to (i.e. PageVersionId or CustomEntityId). This is used to
        /// return the display model back into the block collection in
        /// the correct place.
        /// </summary>
        public int VersionBlockId { get; set; }

        /// <summary>
        /// A mapped display model ready for rendering.
        /// </summary>
        public IPageBlockTypeDisplayModel DisplayModel { get; set; }
    }
}
