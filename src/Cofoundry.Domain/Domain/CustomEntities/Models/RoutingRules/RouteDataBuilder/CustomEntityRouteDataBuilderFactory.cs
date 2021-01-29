using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Factory for creating ICustomEntityRouteDataBuilder instances.
    /// </summary>
    public class CustomEntityRouteDataBuilderFactory : ICustomEntityRouteDataBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomEntityRouteDataBuilderFactory(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Constructs ICustomEntityRouteDataBuilder for specifc custom
        /// entity types.
        /// </summary>
        public IEnumerable<ICustomEntityRouteDataBuilder<TCustomEntityDefinition, TDataModel>> Create<TCustomEntityDefinition, TDataModel>()
            where TCustomEntityDefinition : ICustomEntityDefinition<TDataModel>
            where TDataModel : ICustomEntityDataModel
        {
            return _serviceProvider.GetServices<ICustomEntityRouteDataBuilder<TCustomEntityDefinition, TDataModel>>();
        }
    }
}
