namespace Cofoundry.Core;

/// <summary>
/// Exception to be used when an argument has an empty value (but not <see langword="null"/>)
/// e.g. <see cref="String.Empty"/>.
/// </summary>
public class ArgumentEmptyException : ArgumentException
{
    public ArgumentEmptyException()
        : base()
    {
    }

    public ArgumentEmptyException(string propertyName)
        : base("Value cannot be empty. Property name: " + propertyName, propertyName)
    {
    }
}
