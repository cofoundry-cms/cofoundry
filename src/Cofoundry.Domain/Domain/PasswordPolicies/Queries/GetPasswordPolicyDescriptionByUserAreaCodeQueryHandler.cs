using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Determines if a username is unique within a specific UserArea.
    /// Usernames only have to be unique per user area.
    /// </summary>
    public class GetPasswordPolicyDescriptionByUserAreaCodeQueryHandler
        : IQueryHandler<GetPasswordPolicyDescriptionByUserAreaCodeQuery, PasswordPolicyDescription>
        , IIgnorePermissionCheckHandler
    {
        private readonly IPasswordPolicyService _newPasswordValidationService;

        public GetPasswordPolicyDescriptionByUserAreaCodeQueryHandler(
            IPasswordPolicyService newPasswordValidationService
            )
        {
            _newPasswordValidationService = newPasswordValidationService;
        }

        public Task<PasswordPolicyDescription> ExecuteAsync(GetPasswordPolicyDescriptionByUserAreaCodeQuery query, IExecutionContext executionContext)
        {
            var policy = _newPasswordValidationService.GetDescription(query.UserAreaCode);

            return Task.FromResult(policy);
        }
    }
}
