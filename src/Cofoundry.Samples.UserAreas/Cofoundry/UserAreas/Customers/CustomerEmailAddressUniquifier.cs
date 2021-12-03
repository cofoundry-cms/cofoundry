using Cofoundry.Core;
using Cofoundry.Domain;

namespace Cofoundry.Samples.UserAreas
{
    public class CustomerEmailAddressUniquifier : IEmailAddressUniquifier<CustomerUserArea>
    {
        private readonly IEmailAddressNormalizer _emailAddressNormalizer;

        public CustomerEmailAddressUniquifier(IEmailAddressNormalizer emailAddressNormalizer)
        {
            _emailAddressNormalizer = emailAddressNormalizer;
        }

        public NormalizedEmailAddress UniquifyAsParts(string emailAddress)
        {
            var normalized = _emailAddressNormalizer.NormalizeAsParts(emailAddress);
            return UniquifyAsParts(normalized);
        }

        public NormalizedEmailAddress UniquifyAsParts(NormalizedEmailAddress emailAddressParts)
        {
            const string GMAIL_DOMAIN = "gmail.com";

            if (emailAddressParts == null) return null;

            // merge both gmail domains as they point to the same inbox
            // ignore any plus addressing and remove superflous dots for gmail addresses only
            var uniqueEmail = emailAddressParts
                .MergeDomains(GMAIL_DOMAIN, "googlemail.com")
                .AlterIf(email => email.HasDomain(GMAIL_DOMAIN), email =>
                {
                    return email
                        .WithoutPlusAddressing()
                        .AlterLocal(local => local.Replace(".", string.Empty));
                });

            return uniqueEmail;
        }
    }
}
