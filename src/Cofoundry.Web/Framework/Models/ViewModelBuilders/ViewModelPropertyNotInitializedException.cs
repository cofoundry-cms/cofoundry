namespace Cofoundry.Web;

public class ViewModelPropertyNotInitializedException : Exception
{
    public ViewModelPropertyNotInitializedException() { }

    public ViewModelPropertyNotInitializedException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ViewModelPropertyNotInitializedException"/> instance,
    /// using the <typeparamref name="TViewModel"/> type name and <paramref name="propertyName"/>
    /// to construct the error message.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// View model type containing the property that is throwing the exception.
    /// </typeparam>
    /// <param name="propertyName">The name of the view model property that is throwing the exception.</param>
    public static ViewModelPropertyNotInitializedException Create<TViewModel>(string propertyName)
        where TViewModel : class
    {
        var typeName = typeof(TViewModel).FullName;
        var msg = $"Non-nullable property '{propertyName}' on type {typeName} has not been initialized, did you forget to map it?";
        return new ViewModelPropertyNotInitializedException(msg);
    }
}
