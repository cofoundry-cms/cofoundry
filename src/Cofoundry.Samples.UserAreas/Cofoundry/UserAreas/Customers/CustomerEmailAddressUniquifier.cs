using Cofoundry.Core;
using Cofoundry.Core.Extendable;
using Cofoundry.Domain;

namespace Cofoundry.Samples.UserAreas
{
    public class CustomerEmailAddressUniquifier : EmailAddressUniquifier, IEmailAddressUniquifier<CustomerUserArea>
    {
        public CustomerEmailAddressUniquifier(IEmailAddressNormalizer emailAddressNormalizer)
            : base(emailAddressNormalizer)
        {
        }

        public override NormalizedEmailAddress UniquifyAsParts(NormalizedEmailAddress emailAddressParts)
        {
            const string GMAIL_DOMAIN = "gmail.com";

            var uniqueEmail = base
                .UniquifyAsParts(emailAddressParts)
                // both gmail domains point to the same inbox
                .MergeDomains(GMAIL_DOMAIN, "googlemail.com")
                // ignore plus addressing for uniqueness checks
                .WithoutPlusAddressing()
                // dots are superflous in gmail emails
                .AlterLocalIf(r => r.HasDomain(GMAIL_DOMAIN), r => r.Replace(".", string.Empty));

            return uniqueEmail;
        }
    }
}
