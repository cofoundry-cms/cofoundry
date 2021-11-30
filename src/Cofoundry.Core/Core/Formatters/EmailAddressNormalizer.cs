using System;

namespace Cofoundry.Core.Internal
{
    /// <inheritdoc/>
    public class EmailAddressNormalizer : IEmailAddressNormalizer
    {
        public virtual string Normalize(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress)) return null;

            var parts = emailAddress
                .Trim()
                .Split('@', System.StringSplitOptions.RemoveEmptyEntries);

            // only 1 @ permitted, not at the start or end
            if (parts.Length != 2) return null;

            var local = NormalizeLocal(parts[0]);
            if (string.IsNullOrWhiteSpace(local)) return null;

            var domain = NormalizeDomain(parts[1]);
            if (string.IsNullOrWhiteSpace(domain)) return null;

            return local + '@' + domain;
        }

        /// <summary>
        /// Normalizes the local part of an email (the part before the @)
        /// </summary>
        protected virtual string NormalizeLocal(string local)
        {
            // Note that in practice it's very unlikely that the local part needs to be case-sensitive
            // but technically only the domain part is case-insensitive. A username formatter may choose
            // to be less RFC compliant here, but for email correspondance we should honor the spec.
            return local;
        }

        /// <summary>
        /// Normalizes the domain part of an email (the part after the @)
        /// </summary>
        protected virtual string NormalizeDomain(string domain)
        {
            if (!Uri.TryCreate("http://" + domain, UriKind.Absolute, out var uri))
            {
                return null;
            }

            if (!string.Equals(uri.IdnHost, uri.Host, StringComparison.Ordinal))
            {
                // Normalizing IDNs is currently out of scope.
                return domain;
            }

            return domain.ToLowerInvariant();
        }
    }
}
