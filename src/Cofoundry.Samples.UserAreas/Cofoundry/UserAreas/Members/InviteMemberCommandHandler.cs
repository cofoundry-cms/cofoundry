using Cofoundry.Core.Web;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    public class InviteMemberCommandHandler
    : ICommandHandler<InviteMemberCommand>
    , ISignedInPermissionCheckHandler
    {
        private readonly IAdvancedContentRepository _contentRepository;
        private readonly IAuthorizedTaskTokenUrlHelper _authorizedTaskTokenUrlHelper;
        private readonly ISiteUrlResolver _siteUrlResolver;

        public InviteMemberCommandHandler(
            IAdvancedContentRepository contentRepository,
            IAuthorizedTaskTokenUrlHelper authorizedTaskTokenUrlHelper,
            ISiteUrlResolver siteUrlResolver
            )
        {
            _contentRepository = contentRepository;
            _authorizedTaskTokenUrlHelper = authorizedTaskTokenUrlHelper;
            _siteUrlResolver = siteUrlResolver;
        }

        public async Task ExecuteAsync(InviteMemberCommand command, IExecutionContext executionContext)
        {
            // Create a new task and token. Here we use task data to capture
            // the email address so it can be retrieved later on
            var token = await _contentRepository
                .AuthorizedTasks()
                .AddAsync(new AddAuthorizedTaskCommand()
                {
                    AuthorizedTaskTypeCode = MemberInviteAuthorizedTaskType.Code,
                    UserId = executionContext.UserContext.UserId.Value,
                    TaskData = command.EmailAddressToInvite
                });

            // Here we use IAuthorizedTaskTokenUrlHelper to insert the token into 
            // the url as a query parameter, but you can forma this however you want
            var inviteUrl = _authorizedTaskTokenUrlHelper.MakeUrl("/members/register", token);

            // If you need to make the url absoute, you can use ISiteUrlResolver
            inviteUrl = _siteUrlResolver.MakeAbsolute(inviteUrl);

            // Send email 
            // (ommitted)
        }
    }
}
