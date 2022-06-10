namespace Cofoundry.Core;

/// <summary>
/// The domain name part of an email address (the part after the @).
/// </summary>
public class EmailDomainName
{
    public EmailDomainName(string domain, string idnDomain)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(domain);
        ArgumentEmptyException.ThrowIfNullOrWhitespace(idnDomain);

        Name = domain;
        IdnName = idnDomain;
    }

    /// <summary>
    /// The domain name normalized to lowercase e.g. "example.com".
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The RFC 3490 compliant International Domain Name version of 
    /// <see cref="Name"/>, using Punycode as appropriate.
    /// </summary>
    public string IdnName { get; }

    /// <summary>
    /// Parses a domain name, creating a new <see cref="EmailDomainName"/>
    /// instance. If the name could not be parsed then <see langword="null"/>
    /// is returned.
    /// </summary>
    /// <param name="domainName">
    /// The case-insensitive domain name to parse e.g. "example.com" or "EXAMPLE.COM".
    /// </param>
    public static EmailDomainName Parse(string domainName)
    {
        if (string.IsNullOrWhiteSpace(domainName)) return null;

        // rely on the .NET Uri class to case the host properly and parse the idn domain
        if (!Uri.TryCreate("http://" + domainName.TrimStart('@'), UriKind.Absolute, out var uri))
        {
            return null;
        }

        return new EmailDomainName(uri.Host, uri.IdnHost);
    }

    public override string ToString()
    {
        return Name;
    }
}
