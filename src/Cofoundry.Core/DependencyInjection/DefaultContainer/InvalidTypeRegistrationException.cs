namespace Cofoundry.Core.DependencyInjection;

/// <summary>
/// Thrown when there is an invalid registration configuration
/// for a specific type.
/// </summary>
public class InvalidTypeRegistrationException : Exception
{
    public InvalidTypeRegistrationException(Type type)
        : base(MakeDefaultMessage(type))
    {
        RegisteredType = type;
    }

    public InvalidTypeRegistrationException(Type type, string message)
        : base(message)
    {
        RegisteredType = type;
    }

    public InvalidTypeRegistrationException(Type type, string message, Exception innerException)
        : base(message, innerException)
    {
        RegisteredType = type;
    }

    private static string MakeDefaultMessage(Type type)
    {
        return $"The configuration for type {type.FullName} is invalid.";
    }

    public Type RegisteredType { get; set; }
}
