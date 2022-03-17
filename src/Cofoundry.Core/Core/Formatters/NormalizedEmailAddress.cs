namespace Cofoundry.Core;

/// <summary>
/// Contains the result of an email address normalization process, broken down into 
/// various parts. Designed to be used as part of a multi-step email normalization 
/// and uniquification process.
/// </summary>
/// <inheritdoc/>
public class NormalizedEmailAddress
{
    public NormalizedEmailAddress(string local, EmailDomainName domain)
    {
        if (string.IsNullOrWhiteSpace(local)) throw new ArgumentEmptyException(nameof(local));
        if (domain == null) throw new ArgumentNullException(nameof(domain));

        Local = local;
        Domain = domain;
    }

    /// <summary>
    /// The local part of the email (the part before the @).
    /// </summary>
    public string Local { get; }

    /// <summary>
    /// The domain name part of the email (the part after the @).
    /// </summary>
    public EmailDomainName Domain { get; }

    /// <summary>
    /// Applies the <paramref name="modifier"/> function to the local part of the email address.
    /// </summary>
    /// <param name="modifier">Function to modify the local part of the email address.</param>
    /// <returns>Returns a new <see cref="NormalizedEmailAddress"/> instance reflecting any changes.</returns>
    public NormalizedEmailAddress AlterLocal(Func<string, string> modifier)
    {
        if (modifier == null) throw new ArgumentNullException(nameof(modifier));
        var newLocal = modifier(Local);

        return new NormalizedEmailAddress(
            newLocal,
            Domain
            );
    }

    /// <summary>
    /// Applies the <paramref name="modifier"/> function to the local part of the email address
    /// only if the <paramref name="predicate"/> truth-test is passed.
    /// </summary>
    /// <param name="predicate">A trust-test function to determine if the change should be made.</param>
    /// <param name="modifier">
    /// Function to modify the domain if the <paramref name="predicate"/> returns <see langword="true"/>.
    /// This function passes in the existing domain name and expects a modified version to be returned.
    /// </param>
    /// <returns>
    /// If the locale is updated then a new <see cref="NormalizedEmailAddress"/> instance is returned; 
    /// otherwise the existing instance is returned.
    /// </returns>
    public NormalizedEmailAddress AlterLocalIf(Func<NormalizedEmailAddress, bool> predicate, Func<string, string> modifier)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (modifier == null) throw new ArgumentNullException(nameof(modifier));

        if (!predicate(this)) return this;

        return AlterLocal(modifier);
    }

    /// <summary>
    /// Applies the <paramref name="modifier"/> function to the domain part of the email address.
    /// </summary>
    /// <param name="modifier">
    /// Function to modify the domain name part of the email address. This function passes in the 
    /// existing domain name and expects a modified version to be returned.
    /// </param>
    /// <returns>Returns a new <see cref="NormalizedEmailAddress"/> instance reflecting any changes.</returns>
    public NormalizedEmailAddress AlterDomain(Func<string, string> modifier)
    {
        if (modifier == null) throw new ArgumentNullException(nameof(modifier));
        var newDomainName = modifier(Domain.Name);
        var newDomain = EmailDomainName.Parse(newDomainName);
        if (newDomain == null) throw new InvalidOperationException($"Domain name {newDomain} could not be parsed.");

        return new NormalizedEmailAddress(
            Local,
            newDomain
            );
    }

    /// <summary>
    /// Applies the <paramref name="modifier"/> function to the domain part of the email address
    /// only if the <paramref name="predicate"/> truth-test is passed.
    /// </summary>
    /// <param name="predicate">A trust-test function to determine if the change should be made.</param>
    /// <param name="modifier">Function to modify the domain if the <paramref name="predicate"/> returns <see langword="true"/>.</param>
    /// <returns>
    /// If the domain is updated then a new <see cref="NormalizedEmailAddress"/> instance is returned; 
    /// otherwise the existing instance is returned.
    /// </returns>
    public NormalizedEmailAddress AlterDomainIf(Func<NormalizedEmailAddress, bool> predicate, Func<string, string> modifier)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (modifier == null) throw new ArgumentNullException(nameof(modifier));

        if (!predicate(this)) return this;

        return AlterDomain(modifier);
    }

    /// <summary>
    /// Applies the <paramref name="modifier"/> function to the email address
    /// only if the <paramref name="predicate"/> truth-test is passed.
    /// </summary>
    /// <param name="predicate">A trust-test function to determine if the change should be made.</param>
    /// <param name="modifier">
    /// Function to modify the email address if the <paramref name="predicate"/> returns <see langword="true"/>.
    /// This function passes in the existing domain name and expects a modified version to be returned.
    /// </param>
    /// <returns>
    /// If the email address is updated then a new <see cref="NormalizedEmailAddress"/> instance is returned; 
    /// otherwise the existing instance is returned.
    /// </returns>
    public NormalizedEmailAddress AlterIf(Func<NormalizedEmailAddress, bool> predicate, Func<NormalizedEmailAddress, NormalizedEmailAddress> modifier)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (modifier == null) throw new ArgumentNullException(nameof(modifier));

        if (!predicate(this)) return this;

        return modifier(this);
    }

    /// <summary>
    /// Returns a new <see cref="NormalizedEmailAddress"/> instance with the local part in 
    /// lowercase. Techically the local can be case-sensitive, but in practice is a very rare edge 
    /// case.
    /// </summary>
    /// <returns>Returns a new <see cref="NormalizedEmailAddress"/> instance reflecting any changes.</returns>
    public NormalizedEmailAddress ToLower()
    {
        var newLocal = Local.ToLowerInvariant();

        return new NormalizedEmailAddress(
            newLocal,
            Domain
            );
    }

    /// <summary>
    /// <para>
    /// Returns a new <see cref="NormalizedEmailAddress"/> instance with any plus
    /// addressing alias removed from the local part. A plus addressing alias is
    /// considered any text after the first "+" sign and is a common but non-standard
    /// practice for creating disposable email alias', therefore removing this text may
    /// break real email addresses in edge cases.
    /// </para>
    /// <para>
    /// Alias' should not be removed from a contact email address as it would break a privacy expectation, 
    /// however you may choose to remove them for the purposes of uniqueness checks.
    /// </para>
    /// </summary>
    /// <returns>Returns a new <see cref="NormalizedEmailAddress"/> instance reflecting any changes.</returns>
    public NormalizedEmailAddress WithoutPlusAddressing()
    {
        var aliasIndex = Local.IndexOf('+');
        // if the first character is +, we shouldn't alter anything
        if (aliasIndex < 1) return this;

        var newLocal = Local.Substring(0, aliasIndex);

        return new NormalizedEmailAddress(
            newLocal,
            Domain
            );
    }

    /// <summary>
    /// Indicates if <see cref="Domain"/> appears in the specified
    /// list of <paramref name="domains"/> using a case insensitive check.
    /// </summary>
    /// <param name="domains">
    /// A set of domain names to check e.g. to check for a gmail domain 
    /// you'd need to pass <c>["gmail.com", "googlemail.com"]</c>.
    /// </param>
    public bool HasDomain(params string[] domains)
    {
        return domains.Any(d => Domain.Name.Equals(d, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Merges emails from a range of domain names into a single domain. Useful
    /// if an email provider has multiple email domains that map to the same mailboxs
    /// e.g. addresses for both "gmail.com" and "googlemail.com" map to the same inbox
    /// so these can be merged into one for the purposes of uniqueness checks.
    /// </summary>
    /// <param name="primaryDomain">
    /// The domain name to replace any <paramref name="otherDomains"/>
    /// with e.g. "gmail.com".
    /// </param>
    /// <param name="otherDomains">
    /// A set of domain names to check e.g. to check for a gmail domain 
    /// you'd need to pass <c>["googlemail.com"]</c>.
    /// </param>
    /// <returns>Returns a new <see cref="NormalizedEmailAddress"/> instance reflecting any changes.</returns>
    public NormalizedEmailAddress MergeDomains(string primaryDomain, params string[] otherDomains)
    {
        if (HasDomain(otherDomains))
        {
            var domain = EmailDomainName.Parse(primaryDomain);
            if (domain == null) throw new InvalidOperationException($"Domain name {primaryDomain} could not be parsed.");
            return new NormalizedEmailAddress(Local, domain);
        }

        return this;
    }

    /// <summary>
    /// Returns a string containing the complete email address.
    /// </summary>
    public string ToEmailAddress()
    {
        return Local + '@' + Domain;
    }

    /// <summary>
    /// Returns a string containing the complete email address.
    /// </summary>
    public override string ToString()
    {
        return ToEmailAddress();
    }

    public override bool Equals(object obj)
    {
        return obj is NormalizedEmailAddress parts &&
               Local == parts.Local &&
               Domain == parts.Domain;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Local, Domain);
    }
}
