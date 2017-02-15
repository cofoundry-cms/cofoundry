using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A wrapper object containing a module display module
    /// and an identifier that is used to put it back into 
    /// a collection after mapping.
    /// </summary>
    public class PageModuleDisplayModelMapperOutput
    {
        /// <summary>
        /// The id of the versioned entity to which this module belongs
        /// to (i.e. PageVersionId or CustomEntityId). This is used to
        /// return the display model back into the module collection in
        /// the correct place.
        /// </summary>
        public int VersionModuleId { get; set; }

        /// <summary>
        /// A mapped display model ready for rendering.
        /// </summary>
        public IPageModuleDisplayModel DisplayModel { get; set; }
    }
}
