using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Factory for creating ICustomEntityRouteDataBuilder instances.
    /// </summary>
    public interface ICustomEntityRouteDataBuilderFactory
    {
        /// <summary>
        /// Constructs ICustomEntityRouteDataBuilder for specifc custom
        /// entity types.
        /// </summary>
        IEnumerable<ICustomEntityRouteDataBuilder<TCustomEntityDefinition, TDataModel>> Create<TCustomEntityDefinition, TDataModel>()
            where TCustomEntityDefinition : ICustomEntityDefinition<TDataModel>
            where TDataModel : ICustomEntityDataModel;
    }
}
