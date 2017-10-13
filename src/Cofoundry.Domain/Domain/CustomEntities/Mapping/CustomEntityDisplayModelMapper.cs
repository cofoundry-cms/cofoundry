using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain
{
    public class CustomEntityDisplayModelMapper : ICustomEntityDisplayModelMapper
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly string mapDisplayModelMethodName = nameof(ICustomEntityDisplayModelMapper<ICustomEntityDataModel, ICustomEntityDisplayModel<ICustomEntityDataModel>>.MapDisplayModelAsync);

        public CustomEntityDisplayModelMapper(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TDisplayModel> MapDisplayModelAsync<TDisplayModel>(CustomEntityRenderDetails dataModel)
            where TDisplayModel : ICustomEntityDisplayModel
        {
            var mapperType = typeof(ICustomEntityDisplayModelMapper<,>).MakeGenericType(dataModel.Model.GetType(), typeof(TDisplayModel));
            var mapper = _serviceProvider.GetRequiredService(mapperType);

            var method = mapperType.GetMethod(mapDisplayModelMethodName);

            return (Task<TDisplayModel>)method.Invoke(mapper, new object[] { dataModel, dataModel.Model });
        }
    }
}
