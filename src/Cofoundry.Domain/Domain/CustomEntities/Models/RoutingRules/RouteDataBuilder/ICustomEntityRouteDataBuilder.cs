using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Implementing this interface allows you to customize the AdditionalRoutingData
    /// collection on a CustomEntityVersionRoute object. This is useful if you need to
    /// add routing data that isn't normally available in the cached route object such as
    /// the title of a linked entity. You can then take advantage of this data in an
    /// ICustomEntityRoutingRule to create bespoke routing for a custom entity details page.
    /// </summary>
    /// <typeparam name="TCustomEntityDefinition">The definition type representing the custom entity type.</typeparam>
    /// <typeparam name="TDataModel">Data model type, must inherit from ICustomEntityDataModel.</typeparam>
    public interface ICustomEntityRouteDataBuilder<TCustomEntityDefinition, TDataModel>
        where TCustomEntityDefinition : ICustomEntityDefinition<TDataModel>
        where TDataModel : ICustomEntityDataModel
    {
        /// <summary>
        /// Batch operation to cuztomize the AdditionalRoutingData collection on a batch 
        /// of CustomEntityVersionRoute. When implementing this method, add your additional 
        /// data to the AdditionalRoutingData property of each CustomEntityRouteDataBuilderParameter
        /// instance.
        /// </summary>
        /// <param name="builderParameters">
        /// Collection of routing data to act on. This method will typically
        /// run on an entire set of custom entities for a specific type, so try
        /// to work with data in bulk.
        /// </param>
        Task BuildAsync(IReadOnlyCollection<CustomEntityRouteDataBuilderParameter<TDataModel>> builderParameters);
    }
}
