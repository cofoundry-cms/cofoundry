using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A wrapper object that contains information that can be used to map
    /// an IPageBlockDataModel to an instance of IPageBlockDisplayModel.
    /// </summary>
    /// <typeparam name="TDataModel">The data model type being mapped.</typeparam>
    public class PageBlockDisplayModelMapperInput<TDataModel> where TDataModel : IPageBlockTypeDataModel
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

        /// <summary>
        /// A shorthand way to create an output object once
        /// the display model has been mapped.
        /// </summary>
        /// <param name="displayModel">The fully mapped display model.</param>
        public PageBlockDisplayModelMapperOutput CreateOutput(IPageBlockTypeDisplayModel displayModel)
        {
            if (displayModel == null) throw new ArgumentNullException(nameof(displayModel));
            if (VersionBlockId < 1) throw new ArgumentOutOfRangeException(nameof(VersionBlockId));

            return new PageBlockDisplayModelMapperOutput()
            {
                DisplayModel = displayModel,
                VersionBlockId = VersionBlockId
            };
        }
    }
}
