using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Mapper for mapping typed display models for custom entities.
    /// </summary>
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

        /// <summary>
        /// Maps custom entity render data into a specific custom entity
        /// display model type, which can be used to render out view data.
        /// </summary>
        /// <typeparam name="TDisplayModel">Display model type to map to.</typeparam>
        /// <param name="renderDetails">The custom entity model to map, including the raw data model.</param>
        /// <param name="publishStatusQuery">
        /// The query type that should be used to query dependent entities. E.g. if the custom
        /// entity was queried with the Published status query, then any dependent entities should
        /// also be queried as Published.
        /// </param>
        /// <returns>Mapped display model instance.</returns>
        public Task<TDisplayModel> MapDisplayModelAsync<TDisplayModel>(
            CustomEntityRenderDetails renderDetails,
            PublishStatusQuery publishStatusQuery
            )
            where TDisplayModel : ICustomEntityDisplayModel
        {
            var mapperType = typeof(ICustomEntityDisplayModelMapper<,>).MakeGenericType(renderDetails.Model.GetType(), typeof(TDisplayModel));
            var mapper = _serviceProvider.GetRequiredService(mapperType);

            var method = mapperType.GetMethod(mapDisplayModelMethodName);

            return (Task<TDisplayModel>)method.Invoke(mapper, new object[] { renderDetails, renderDetails.Model, publishStatusQuery });
        }
    }
}
