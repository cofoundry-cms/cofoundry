using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A wrapper object that contains information that can be used to map
    /// an IPageBlockTypeDataModel to an instance of IPageBlockTypeDisplayModel.
    /// </summary>
    /// <typeparam name="TDataModel">The data model type being mapped.</typeparam>
    public class PageBlockTypeDisplayModelMapperInput<TDataModel> where TDataModel : IPageBlockTypeDataModel
    {
        /// <summary>
        /// The id of the versioned entity to which this block belongs
        /// to (i.e. PageVersionId or CustomEntityId). This is used to
        /// return the display model back into the block collection in
        /// the correct place.
        /// </summary>
        public int VersionBlockId { get; set; }

        /// <summary>
        /// The data model to map to a display model.
        /// </summary>
        public TDataModel DataModel { get; set; }
    }
}
