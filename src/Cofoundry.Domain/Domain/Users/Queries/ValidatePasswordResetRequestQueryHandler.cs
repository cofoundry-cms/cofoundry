using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class ValidatePasswordResetRequestQueryHandler
        : IAsyncQueryHandler<ValidatePasswordResetRequestQuery, PasswordResetRequestAuthenticationResult>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly AuthenticationSettings _authenticationSettings;

        public ValidatePasswordResetRequestQueryHandler(
            CofoundryDbContext dbContext,
            IUserAreaDefinitionRepository userAreaRepository,
            AuthenticationSettings authenticationSettings
            )
        {
            _dbContext = dbContext;
            _userAreaRepository = userAreaRepository;
            _authenticationSettings = authenticationSettings;
        }
        
        #endregion

        public async Task<PasswordResetRequestAuthenticationResult> ExecuteAsync(ValidatePasswordResetRequestQuery query, IExecutionContext executionContext)
        {
            var request = await _dbContext
                .UserPasswordResetRequests
                .AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.UserPasswordResetRequestId == query.UserPasswordResetRequestId)
                .SingleOrDefaultAsync();

            var result = ValidatePasswordRequest(request, query, executionContext);

            return result;
        }

        private PasswordResetRequestAuthenticationResult ValidatePasswordRequest(UserPasswordResetRequest request, ValidatePasswordResetRequestQuery query, IExecutionContext executionContext)
        {
            var result = new PasswordResetRequestAuthenticationResult();

            if (request == null || request.Token != query.Token)
            {
                result.Error = PasswordResetRequestAuthenticationError.InvalidRequest;
                return result;
            }

            if (request.User.UserAreaCode != query.UserAreaCode)
            {
                throw new InvalidPasswordResetRequestException(query, "Request received through an invalid route (incorrect user area)");
            }

            if (request.User.IsDeleted || request.User.IsSystemAccount)
            {
                throw new InvalidPasswordResetRequestException(query, "User not permitted to change password");
            }

            var userArea = _userAreaRepository.GetByCode(request.User.UserAreaCode);

            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidPasswordResetRequestException(query, "Cannot update the password to account in a user area that does not allow password logins.");
            }

            if (request.IsComplete)
            {
                result.Error = PasswordResetRequestAuthenticationError.AlreadyComplete;
                return result;
            }

            if (!IsPasswordRecoveryDateValid(request.CreateDate, executionContext))
            {
                result.Error = PasswordResetRequestAuthenticationError.Expired;
                return result;
            }

            result.IsValid = true;
            return result;
        }

        private bool IsPasswordRecoveryDateValid(DateTime dt, IExecutionContext executionContext)
        {
            return dt > executionContext.ExecutionDate.AddHours(-_authenticationSettings.NumHoursPasswordResetLinkValid);
        }
    }

}
