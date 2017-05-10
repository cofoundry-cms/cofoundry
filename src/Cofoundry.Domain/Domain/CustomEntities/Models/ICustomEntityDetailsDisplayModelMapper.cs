using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface ICustomEntityDetailsDisplayModelMapper<TDataModel, TDisplayModel>
        where TDataModel : ICustomEntityVersionDataModel
        where TDisplayModel : ICustomEntityDisplayModel<TDataModel>
    {
        Task<TDisplayModel> MapDetailsAsync(CustomEntityRenderDetails renderDetails, TDataModel dataModel);
    }
}
