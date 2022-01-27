using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Validates a user email address, returning any errors found. By default the validator
    /// checks that the format contains only the characters permitted by the 
    /// <see cref="EmailAddressOptions"/> configuration settings, as well as checking
    /// for uniqueness if necessary.
    /// </summary>
    public class ValidateUserEmailAddressQueryHandler
        : IQueryHandler<ValidateUserEmailAddressQuery, ValidationQueryResult>
        , IPermissionRestrictedQueryHandler<ValidateUserEmailAddressQuery, ValidationQueryResult>
    {
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserDataFormatter _userDataFormatter;
        private readonly IEmailAddressValidator _emailAddressValidator;

        public ValidateUserEmailAddressQueryHandler(
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserDataFormatter userDataFormatter,
            IEmailAddressValidator emailAddressValidator
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userDataFormatter = userDataFormatter;
            _emailAddressValidator = emailAddressValidator;
        }

        public async Task<ValidationQueryResult> ExecuteAsync(ValidateUserEmailAddressQuery query, IExecutionContext executionContext)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(query.UserAreaCode);
            var formattingResult = _userDataFormatter.FormatEmailAddress(userArea, query.Email);

            var context = new EmailAddressValidationContext()
            {
                Email = formattingResult,
                ExecutionContext = executionContext,
                PropertyName = nameof(query.Email),
                UserAreaCode = userArea.UserAreaCode,
                UserId = query.UserId
            };

            var errors = await _emailAddressValidator.GetErrorsAsync(context);
            var result = new ValidationQueryResult(errors.FirstOrDefault());

            return result;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(ValidateUserEmailAddressQuery query)
        {
            if (query.UserAreaCode == CofoundryAdminUserArea.Code)
            {
                yield return new CofoundryUserReadPermission();
            }
            else
            {
                yield return new NonCofoundryUserReadPermission();
            }
        }
    }
}
