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

        public CustomEntityDisplayModelMapper(
            IResolutionContext resolutionContext
            )
        {
            _resolutionContext = resolutionContext;
        }

        public TDisplayModel MapDetails<TDisplayModel>(CustomEntityRenderDetails dataModel)
            where TDisplayModel : ICustomEntityDisplayModel
        {
            var mapperType = typeof(ICustomEntityDetailsDisplayModelMapper<,>).MakeGenericType(dataModel.Model.GetType(), typeof(TDisplayModel));
            var mapper = _resolutionContext.Resolve(mapperType);

            var method = mapperType.GetMethod("MapDetails");

            return (TDisplayModel)method.Invoke(mapper, new object[] { dataModel, dataModel.Model });
        }
    }
}
