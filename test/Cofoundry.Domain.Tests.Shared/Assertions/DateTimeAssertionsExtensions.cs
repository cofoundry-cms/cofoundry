using FluentAssertions.Primitives;

namespace Cofoundry.Domain.Tests.Shared.Assertions;

public static class DateTimeAssertionsExtensions
{
    /// <summary>
    /// Asserts that the <see cref="DateTime"/> instance is not the
    /// default value.
    /// </summary>
    /// <param name="because">
    /// A formatted phrase as is supported by System.String.Format(System.String,System.Object[])
    /// explaining why the assertion is needed. If the phrase does not start with the
    /// word because, it is prepended automatically.
    ///</param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in because.
    /// </param>
    public static AndConstraint<TAssertions> NotBeDefault<TAssertions>(this DateTimeAssertions<TAssertions> parent, string because = "", params object[] becauseArgs)
        where TAssertions : DateTimeAssertions<TAssertions>
    {
        return parent.NotBe(default(DateTime), because, becauseArgs);
    }
}
