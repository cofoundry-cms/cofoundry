using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A wrapper object that contains information that can be used to map
    /// an IPageModuleDataModel to an instance of IPageModuleDisplayModel.
    /// </summary>
    /// <typeparam name="TDataModel">The data model type being mapped.</typeparam>
    public class PageModuleDisplayModelMapperInput<TDataModel> where TDataModel : IPageModuleDataModel
    {
        /// <summary>
        /// The id of the versioned entity to which this module belongs
        /// to (i.e. PageVersionId or CustomEntityId). This is used to
        /// return the display model back into the module collection in
        /// the correct place
        /// </summary>
        public int VersionModuleId { get; set; }

        /// <summary>
        /// The dat model to map to a display model.
        /// </summary>
        public TDataModel DataModel { get; set; }

        /// <summary>
        /// A shorthand way to create an output object once
        /// the display model has been mapped.
        /// </summary>
        /// <param name="displayModel">The fully mapped display model</param>
        public PageModuleDisplayModelMapperOutput CreateOutput(IPageModuleDisplayModel displayModel)
        {
            if (displayModel == null) throw new ArgumentNullException(nameof(displayModel));
            if (VersionModuleId < 1) throw new ArgumentOutOfRangeException(nameof(VersionModuleId));

            return new PageModuleDisplayModelMapperOutput()
            {
                DisplayModel = displayModel,
                VersionModuleId = VersionModuleId
            };
        }
    }
}
