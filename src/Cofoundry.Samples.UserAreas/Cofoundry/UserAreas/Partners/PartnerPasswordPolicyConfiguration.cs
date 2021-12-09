using Cofoundry.Domain;

namespace Cofoundry.Samples.UserAreas
{
    public class PartnerPasswordPolicyConfiguration : IPasswordPolicyConfiguration<PartnerUserArea>
    {
        public void Configure(IPasswordPolicyBuilder builder)
        {
            builder.UseDefaults(c => c.MinLength = 12);
        }
    }
}
