namespace Cofoundry.Domain.CQS;

/// <summary>
/// An exception for when a handler cannot be found for an <see cref="IQueryHandler{TQuery, TResult}"/> 
/// or <see cref="ICommand"/>. Typically this means a problem with handler registration.
/// </summary>
public class MissingHandlerMappingException : Exception
{
    public MissingHandlerMappingException()
    {
    }

    public MissingHandlerMappingException(string message)
        : base(message)
    {
    }

    public MissingHandlerMappingException(Type t)
        : base(FormatMessage(t))
    {
    }

    private static string FormatMessage(Type t)
    {
        const string ERROR_MESSAGE = "Could not locate a handler for type: '{0}'.";

        if (t == null)
        {
            return string.Format(CultureInfo.InvariantCulture, ERROR_MESSAGE, "NULL");
        }

        return string.Format(CultureInfo.InvariantCulture, ERROR_MESSAGE, t.FullName);
    }
}
