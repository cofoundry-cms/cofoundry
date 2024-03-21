using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Cofoundry.Core;

/// <summary>
/// Exception to be used when an argument has an empty value (but not <see langword="null"/>)
/// e.g. <see cref="string.Empty"/>.
/// </summary>
public class ArgumentEmptyException : ArgumentException
{
    public ArgumentEmptyException()
        : base()
    {
    }

    public ArgumentEmptyException(string? paramName)
        : this(paramName, null)
    {
    }

    public ArgumentEmptyException(string? paramName, string? message)
        : base(paramName, message ?? $"Argument '{paramName ?? "not specified"}' cannot be empty.")
    {
    }

    /// <summary>
    /// Throws an <see cref="ArgumentEmptyException"/> if <paramref name="argument"/>
    /// contains no elements. Throws an <see cref="ArgumentNullException"/> if the
    /// argument is <see langword="null"/>.
    /// </summary>
    /// <param name="argument">Argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfNullOrEmpty<T>([NotNull] IReadOnlyCollection<T>? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (argument.Count == 0)
        {
            throw new ArgumentEmptyException(null, paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentEmptyException"/> if <paramref name="argument"/>
    /// contains no elements. Throws an <see cref="ArgumentNullException"/> if the
    /// argument is <see langword="null"/>.
    /// </summary>
    /// <param name="argument">Argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfNullOrEmpty<T>([NotNull] IEnumerable<T>? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(null, paramName);
        }

        if (!argument.Any())
        {
            throw new ArgumentEmptyException(null, paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentEmptyException"/> if <paramref name="argument"/>
    /// is the default value e.g. for a <see cref="DateTime"/>, <see cref="DateTime.MinValue"/> is
    /// considered empty.
    /// </summary>
    /// <param name="argument">Argument to validate.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfDefault<T>(T argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : struct
    {
        if (argument.Equals(default(T)))
        {
            throw new ArgumentEmptyException(null, paramName);
        }
    }
}
