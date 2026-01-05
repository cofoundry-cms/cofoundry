namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IEmptyDataModelFactory"/>.
/// </summary>
public static class IEmptyDataModelFactoryExtensions
{
    extension(IEmptyDataModelFactory emptyDataModelFactory)
    {
        /// <summary>
        /// Create an empty instance of an entity data model of type
        /// <typeparamref name="TConcrete"/>. This is expected to be
        /// used in mapping when there is a problem deserializing an existing 
        /// data model so that we don't ever have to return <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TConcrete">
        /// The concrete data model type to create e.g. "ExampleBlogDataModel". All data
        /// model types should support a public parameterless constructor, otherwise an
        /// <see cref="Exception"/> will be thrown.
        /// </typeparam>
        /// <returns>
        /// Newly created empty instance of <typeparamref name="TConcrete"/>.
        /// </returns>
        public TConcrete Create<TConcrete>()
            where TConcrete : class
        {
            return emptyDataModelFactory.Create<TConcrete>(typeof(TConcrete));
        }
    }
}
