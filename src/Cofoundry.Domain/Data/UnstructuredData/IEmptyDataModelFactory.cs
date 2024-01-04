namespace Cofoundry.Domain.Data;

/// <summary>
/// Used to create empty instances of entity data models such as
/// <see cref="ICustomEntityDataModel"/>. This is expected to be
/// used in mapping when there is a problem deserializing an existing 
/// data model so that we don't ever have to return null.
/// </summary>
public interface IEmptyDataModelFactory
{
    /// <summary>
    /// Create an empty instance of an entity data model of type
    /// <paramref name="concreteType"/>, which should inherit from
    /// <typeparamref name="TInterface"/>, e.g. 'ExampleBlogDataModel'
    /// which inherits from <see cref="ICustomEntityDataModel"/>. This is expected to be
    /// used in mapping when there is a problem deserializing an existing 
    /// data model so that we don't ever have to return null.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The return data model interface type that <paramref name="concreteType"/> 
    /// should implement e.g. <see cref="ICustomEntityDataModel"/>. An <see cref="Exception"/> 
    /// will be thrown if <paramref name="concreteType"/> does not implement 
    /// <typeparamref name="TInterface"/>.
    /// </typeparam>
    /// <param name="concreteType">
    /// The concrete data model type to create e.g. "ExampleBlogDataModel". All data
    /// model types should support a public parameterless constructor, otherwise an
    /// <see cref="Exception"/> will be thrown.
    /// </param>
    /// <returns>
    /// Newly created empty instance of <paramref name="concreteType"/> cast to 
    /// <typeparamref name="TInterface"/>.
    /// </returns>
    TInterface Create<TInterface>(Type concreteType)
        where TInterface : class;
}
