using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface ICustomEntityDisplayModelMapper<TDataModel, TDisplayModel>
        where TDataModel : ICustomEntityDataModel
        where TDisplayModel : ICustomEntityDisplayModel<TDataModel>
    {
        Task<TDisplayModel> MapDisplayModelAsync(CustomEntityRenderDetails renderDetails, TDataModel dataModel);
    }
}
