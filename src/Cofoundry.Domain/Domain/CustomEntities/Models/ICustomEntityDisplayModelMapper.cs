using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// When implemented, defines a mapper that can translate the raw custom entity
    /// data model into a display model that is rendered out to a view template. This
    /// is particularly useful for fetching related entities that are typically only 
    /// defined as keys on the data model.
    /// </summary>
    /// <typeparam name="TDataModel">The raw data model type to map from.</typeparam>
    /// <typeparam name="TDisplayModel">The display model type to create and map to.</typeparam>
    public interface ICustomEntityDisplayModelMapper<TDataModel, TDisplayModel>
        where TDataModel : ICustomEntityDataModel
        where TDisplayModel : ICustomEntityDisplayModel<TDataModel>
    {
        /// <summary>
        /// Maps a raw custom entity data model to a display model that can be rendered out 
        /// to a view template.
        /// </summary>
        /// <param name="renderDetails">The raw custom entity data pulled from the database.</param>
        /// <param name="dataModel">
        /// Typed model data to map from. This is the same model that's in the render 
        /// details model, but is passed in as a typed model for easy access.
        /// </param>
        /// <param name="publishStatusQuery">
        /// The query type that should be used to query dependent entities. E.g. if the custom
        /// entity was queried with the Published status query, then any dependent entities should
        /// also be queried as Published.
        /// </param>
        /// <returns>Mapped display model instance.</returns>
        Task<TDisplayModel> MapDisplayModelAsync(
            CustomEntityRenderDetails renderDetails, 
            TDataModel dataModel,
            PublishStatusQuery publishStatusQuery
            );
    }
}
