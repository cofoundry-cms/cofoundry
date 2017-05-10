using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using System.Reflection;

namespace Cofoundry.Domain
{
    public class CustomEntityDisplayModelMapper : ICustomEntityDisplayModelMapper
    {
        private readonly IResolutionContext _resolutionContext;
        private static readonly string mapDetailsMethodName = nameof(ICustomEntityDetailsDisplayModelMapper<ICustomEntityVersionDataModel, ICustomEntityDisplayModel<ICustomEntityVersionDataModel>>.MapDetailsAsync);

        public CustomEntityDisplayModelMapper(
            IResolutionContext resolutionContext
            )
        {
            _resolutionContext = resolutionContext;
        }

        public Task<TDisplayModel> MapDetailsAsync<TDisplayModel>(CustomEntityRenderDetails dataModel)
            where TDisplayModel : ICustomEntityDisplayModel
        {
            var mapperType = typeof(ICustomEntityDetailsDisplayModelMapper<,>).MakeGenericType(dataModel.Model.GetType(), typeof(TDisplayModel));
            var mapper = _resolutionContext.Resolve(mapperType);

            var method = mapperType.GetMethod(mapDetailsMethodName);

            return (Task<TDisplayModel>)method.Invoke(mapper, new object[] { dataModel, dataModel.Model });
        }
    }
}
