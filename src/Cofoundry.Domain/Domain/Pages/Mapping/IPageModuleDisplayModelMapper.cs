using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A custom mapper where an IPageModuleDisplayModel has more advanced requirements
    /// then just returning the bare IPageModuleDataModel. Mapping is done in batches for
    /// efficiency purposes.
    /// </summary>
    public interface IPageModuleDisplayModelMapper<TDataModel> where TDataModel : IPageModuleDataModel
    {
        IEnumerable<PageModuleDisplayModelMapperOutput> Map(IEnumerable<PageModuleDisplayModelMapperInput<TDataModel>> inputs, WorkFlowStatusQuery workflowStatus);
    }
}
