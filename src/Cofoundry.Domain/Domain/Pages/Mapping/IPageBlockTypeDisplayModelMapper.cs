using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A custom mapper where an IPageBlockTypeDisplayModel has more advanced requirements
    /// then just returning the bare IPageBlockDataModel. Mapping is done in batches for
    /// efficiency purposes.
    /// </summary>
    public interface IPageBlockTypeDisplayModelMapper<TDataModel> where TDataModel : IPageBlockTypeDataModel
    {
        /// <summary>
        /// Maps a batch of raw page block data to a set of display models. Once you've mapped 
        /// a display model, you can use input.CreateOutput to turn it into the return type.
        /// </summary>
        /// <param name="inputCollection">Raw page block data input.</param>
        /// <param name="publishStatus">
        /// A modified version of the parent page publish status suitable to be used 
        /// to query dependent entities. This isn't necessarily the same as the parent page
        /// status, E.g. PublishStatusQuery.SpecificVersion cannot be used to query a 
        /// dependent entity and so PublishStatusQuery.Latest is used instead.
        /// </param>
        /// <returns>Collection of mapped display models, wrapped in PageBlockTypeDisplayModelMapperOutput objects.</returns>
        Task<IEnumerable<PageBlockTypeDisplayModelMapperOutput>> MapAsync(IReadOnlyCollection<PageBlockTypeDisplayModelMapperInput<TDataModel>> inputCollection, PublishStatusQuery publishStatus);
    }
}
