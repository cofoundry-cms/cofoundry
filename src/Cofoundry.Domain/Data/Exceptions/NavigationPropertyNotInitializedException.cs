namespace Cofoundry.Domain.Data;

/// <summary>
/// Thrown when the getter on a required (not nullable) navigation property in an 
/// entity framework model is called but has not been initialized e.g. not included 
/// in an "Include" statement in a query.
/// </summary>
public class NavigationPropertyNotInitializedException : Exception
{
    public NavigationPropertyNotInitializedException() { }

    public NavigationPropertyNotInitializedException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="NavigationPropertyNotInitializedException"/> instance,
    /// using the <typeparamref name="TModel"/> type name and <paramref name="propertyName"/>
    /// to construct the error message.
    /// </summary>
    /// <typeparam name="TModel">
    /// Entity framework model type containing the property that is throwing the exception.
    /// </typeparam>
    /// <param name="propertyName">The name of the model property that is throwing the exception.</param>
    public static NavigationPropertyNotInitializedException Create<TModel>(string propertyName)
        where TModel : class
    {
        var typeName = typeof(TModel).FullName;
        var msg = $"Non-nullable property '{propertyName}' on type {typeName} has not been initialized, did you forget to include it in an entity framework query?";
        return new NavigationPropertyNotInitializedException(msg);
    }
}
