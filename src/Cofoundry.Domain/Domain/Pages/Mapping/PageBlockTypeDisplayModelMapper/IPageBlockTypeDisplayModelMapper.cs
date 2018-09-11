using Cofoundry.Domain.CQS;
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
        /// Maps a batch of raw page block data to a set of display models. The context.Items
        /// property contains the items to be mapped and each mapped item should be added to
        /// the result parameter by calling result.Add(inputDataModel, mappedDisplayModel).
        /// </summary>
        /// <param name="context">
        /// Contains the collection of data model items to be mapped
        /// as well as contextual information that might be useful
        /// during the mapping process such as a PublishStatusQuery
        /// and ExecutionContext that can be used when quering related
        /// entities.
        /// </param>
        /// <param name="result">
        /// A container for the result of this mapping operation. Each mapped 
        /// item should be added to the result parameter by calling 
        /// result.Add(inputDataModel, mappedDisplayModel). If the item 
        /// cannot be mapped (e.g. missing related data) then do not add
        /// it to the result collection.
        /// </param>
        Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<TDataModel> context,
            PageBlockTypeDisplayModelMapperResult<TDataModel> result
            );
    }
}
