using FluentAssertions.Primitives;

namespace Cofoundry.Domain.Tests.Shared.Assertions;

/// <summary>
/// Fluent assertion extensions for <see cref="DateTime"/>.
/// </summary>
public static class DateTimeAssertionsExtensions
{
    extension<TAssertions>(DateTimeAssertions<TAssertions> parent) where TAssertions : DateTimeAssertions<TAssertions>
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
        public AndConstraint<TAssertions> NotBeDefault(string because = "", params object[] becauseArgs)
        {
            return parent.NotBe(default, because, becauseArgs);
        }
    }
}
