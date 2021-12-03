namespace Cofoundry.Core.Extendable
{
    /// <inheritdoc/>
    public class EmailAddressUniquifier : IEmailAddressUniquifier
    {
        private readonly IEmailAddressNormalizer _emailAddressNormalizer;

        public EmailAddressUniquifier(
            IEmailAddressNormalizer emailAddressNormalizer
            )
        {
            _emailAddressNormalizer = emailAddressNormalizer;
        }

        public virtual NormalizedEmailAddress UniquifyAsParts(string emailAddress)
        {
            var parts = _emailAddressNormalizer.NormalizeAsParts(emailAddress);
            return UniquifyAsParts(parts);
        }

        public virtual NormalizedEmailAddress UniquifyAsParts(NormalizedEmailAddress emailAddressParts)
        {
            return emailAddressParts?.ToLower();
        }
    }
}
