namespace Cofoundry.Core;

/// <summary>
/// Used to format an email address into a value that can be used for
/// uniquness comparisons. The default implementation simply lowercases
/// the email, but this can be overriden to provide more strict uniqueness
/// checks.
/// </summary>
public interface IEmailAddressUniquifier
{
    /// <summary>
    /// <para>
    /// Formats an email address into a value that can be used for
    /// uniqueness comparisons. The default implementation simply lowercases
    /// the email, but this can be overriden to provide more strict uniqueness
    /// checks.
    /// </para>
    /// <para>
    /// As an example, if the user types their email as &quot; L.Balfour+cofoundry@Example.com&quot; then by
    /// default the email would normalize via <see cref="IEmailAddressNormalizer"/> 
    /// as &quot;L.Balfour+cofoundry@example.com&quot; and uniquify as &quot;l.balfour+cofoundry@example.com&quot;,
    /// however if you wanted to exclude &quot;plus addressing&quot; from email uniqueness checks, you could 
    /// override the default <see cref="IEmailAddressUniquifier"/> implementation with your own
    /// custom implementation. 
    /// </para>
    /// </summary>
    /// <param name="emailAddress">
    /// The email address to uniquify. If the value is <see langword="null"/> 
    /// then <see langword="null"/> is returned.
    /// </param>
    NormalizedEmailAddress? UniquifyAsParts(string? emailAddress);

    /// <summary>
    /// Format an email address into a value that can be used for
    /// uniqueness comparisons. The default implementation simply lowercases
    /// the email, but this can be overriden to provide more strict uniqueness
    /// checks.
    /// </summary>
    /// <param name="emailAddressParts">
    /// The pre-normalized email address to uniquify. If the value is <see langword="null"/> 
    /// then <see langword="null"/> is returned.
    /// </param>
    NormalizedEmailAddress? UniquifyAsParts(NormalizedEmailAddress? emailAddressParts);
}
