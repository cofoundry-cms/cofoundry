using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// A simple command handler to wrap member sign out logic. Although it's 
    /// only a one-liner, we've created a handler just to keep it consistent
    /// with the reset of the domain logic.
    /// </summary>
    public class SignOutMemberCommandHandler
        : ICommandHandler<SignOutMemberCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IAdvancedContentRepository _contentRepository;

        public SignOutMemberCommandHandler(
            IAdvancedContentRepository contentRepository
            )
        {
            _contentRepository = contentRepository;
        }

        public Task ExecuteAsync(SignOutMemberCommand command, IExecutionContext executionContext)
        {
            return _contentRepository
                .Users()
                .Authentication()
                .SignOutAsync();
        }
    }
}