using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Validates a username, returning any errors found. By default the validator checks that 
    /// the format contains only the characters permitted by the <see cref="UsernameOptions"/> 
    /// configuration settings, as well as checking for uniquness.
    /// </summary>
    public class ValidateUsernameQueryHandler
        : IQueryHandler<ValidateUsernameQuery, ValidationQueryResult>
        , IPermissionRestrictedQueryHandler<ValidateUsernameQuery, ValidationQueryResult>
    {
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserDataFormatter _userDataFormatter;
        private readonly IUsernameValidator _usernameValidator;

        public ValidateUsernameQueryHandler(
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserDataFormatter userDataFormatter,
            IUsernameValidator usernameValidator
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userDataFormatter = userDataFormatter;
            _usernameValidator = usernameValidator;
        }

        public async Task<ValidationQueryResult> ExecuteAsync(ValidateUsernameQuery query, IExecutionContext executionContext)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(query.UserAreaCode);
            var formattingResult = _userDataFormatter.FormatUsername(userArea, query.Username);

            var context = new UsernameValidationContext()
            {
                Username = formattingResult,
                ExecutionContext = executionContext,
                PropertyName = nameof(query.Username),
                UserAreaCode = userArea.UserAreaCode,
                UserId = query.UserId
            };

            var errors = await _usernameValidator.GetErrorsAsync(context);
            var result = new ValidationQueryResult(errors.FirstOrDefault());

            return result;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(ValidateUsernameQuery query)
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
