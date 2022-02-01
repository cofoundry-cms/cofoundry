using Cofoundry.Domain.CQS;

namespace Cofoundry.Samples.UserAreas
{
    public class InviteMemberCommand : ICommand
    {
        public string EmailAddressToInvite { get; set; }
    }
}
