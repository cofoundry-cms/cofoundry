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
        Task<IEnumerable<PageBlockTypeDisplayModelMapperOutput>> MapAsync(IEnumerable<PageBlockTypeDisplayModelMapperInput<TDataModel>> inputs, WorkFlowStatusQuery workflowStatus);
    }
}
