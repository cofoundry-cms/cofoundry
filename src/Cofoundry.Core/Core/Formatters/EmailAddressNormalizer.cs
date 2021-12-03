namespace Cofoundry.Core.Extendable
{
    /// <inheritdoc/>
    public class EmailAddressNormalizer : IEmailAddressNormalizer
    {
        public virtual NormalizedEmailAddress NormalizeAsParts(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress)) return null;

            var parts = emailAddress
                .Trim()
                .Split('@', System.StringSplitOptions.RemoveEmptyEntries);

            // only 1 @ permitted, not at the start or end
            if (parts.Length != 2) return null;

            // Note that in practice it's very unlikely that the local part needs to be case-sensitive
            // but technically only the domain part is case-insensitive. A username formatter may choose
            // to be less RFC compliant here, but for email correspondance we should honor the spec.
            var local = parts[0];

            var domain = EmailDomainName.Parse(parts[1]);
            var result = new NormalizedEmailAddress(local, domain);

            return result;
        }
    }
}
