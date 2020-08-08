using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class ValidatePasswordResetRequestQueryHandler
        : IQueryHandler<ValidatePasswordResetRequestQuery, PasswordResetRequestAuthenticationResult>
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

        #region execution

        public async Task<PasswordResetRequestAuthenticationResult> ExecuteAsync(ValidatePasswordResetRequestQuery query, IExecutionContext executionContext)
        {
            var request = await GetRequest(query).SingleOrDefaultAsync();
            var result = ValidatePasswordRequest(request, query, executionContext);

            return result;
        }

        #endregion

        #region helpers

        private IQueryable<UserPasswordResetRequest> GetRequest(ValidatePasswordResetRequestQuery query)
        {
            return _dbContext
                .UserPasswordResetRequests
                .AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.UserPasswordResetRequestId == query.UserPasswordResetRequestId);
        }

        private PasswordResetRequestAuthenticationResult ValidatePasswordRequest(UserPasswordResetRequest request, ValidatePasswordResetRequestQuery query, IExecutionContext executionContext)
        {
            if (request == null || request.Token != query.Token)
            {
                throw new InvalidPasswordResetRequestException(query, "Invalid password request - Id: " + query.UserPasswordResetRequestId + " Token: " + query.Token);
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

            var result = new PasswordResetRequestAuthenticationResult();
            result.IsValid = true;

            if (request.IsComplete)
            {
                result.IsValid = false;
                result.ValidationErrorMessage = "The password recovery request is no longer valid.";
            }

            if (!IsPasswordRecoveryDateValid(request.CreateDate, executionContext))
            {
                result.IsValid = false;
                result.ValidationErrorMessage = "The password recovery request has expired.";
            }

            return result;
        }

        private bool IsPasswordRecoveryDateValid(DateTime dt, IExecutionContext executionContext)
        {
            return dt > executionContext.ExecutionDate.AddHours(-_authenticationSettings.NumHoursPasswordResetLinkValid);
        }


        #endregion
    }

}
