namespace Cofoundry.Web.Framework.Mvc.ViewHelpers;

public class ViewHelperNotContextualizedException : Exception
{
    public ViewHelperNotContextualizedException() { }

    public ViewHelperNotContextualizedException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ViewHelperNotContextualizedException"/> instance,
    /// using the <typeparamref name="TViewHelper"/> type name and <paramref name="memberName"/>
    /// to construct the error message.
    /// </summary>
    /// <typeparam name="TViewHelper">
    /// View model type containing the property that is throwing the exception.
    /// </typeparam>
    /// <param name="memberName">The name of the view model property that is throwing the exception.</param>
    public static ViewHelperNotContextualizedException Create<TViewHelper>(string memberName)
        where TViewHelper : class
    {
        var typeName = typeof(TViewHelper).FullName;
        var msg = $"'{memberName}' on view helper '{typeName}' cannot be invoked before the view helper has not been contextualized.";
        return new ViewHelperNotContextualizedException(msg);
    }
}
