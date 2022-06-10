using System.Runtime.CompilerServices;

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

    public ArgumentEmptyException(string argumentName)
        : base($"Argument '{argumentName ?? "not specified"}' cannot be empty.", argumentName)
    {
    }

    /// <summary>
    /// Throws an <see cref="ArgumentEmptyException"/> if <paramref name="argument"/>
    /// is empty or whitespace. Throws an <see cref="ArgumentNullException"/> if the
    /// argument is <see langword="null"/>.
    /// </summary>
    /// <param name="argument">Argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfNullOrWhitespace(string argument, [CallerArgumentExpression("argument")] string paramName = null)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentEmptyException(paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentEmptyException"/> if <paramref name="argument"/>
    /// is the default value e.g. for a <see cref="DateTime"/>, <see cref="DateTime.MinValue"/> is
    /// considered empty.
    /// </summary>
    /// <param name="argument">Argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfDefault<T>(T argument, [CallerArgumentExpression("argument")] string paramName = null)
        where T : struct
    {
        if (argument.Equals(default(T)))
        {
            throw new ArgumentEmptyException(paramName);
        }
    }
}
