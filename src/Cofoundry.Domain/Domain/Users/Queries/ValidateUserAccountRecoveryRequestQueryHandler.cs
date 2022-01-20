using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Determines if an account recovery request is valid. The result is returned as a 
    /// <see cref="ValidationQueryResult"/> which describes any errors that have occured.
    /// </summary>
    public class ValidateUserAccountRecoveryRequestQueryHandler
        : IQueryHandler<ValidateUserAccountRecoveryRequestQuery, ValidationQueryResult>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserAccountRecoveryTokenFormatter _userAccountRecoveryTokenFormatter;

        public ValidateUserAccountRecoveryRequestQueryHandler(
            CofoundryDbContext dbContext,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserAccountRecoveryTokenFormatter userAccountRecoveryTokenFormatter
            )
        {
            _dbContext = dbContext;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userAccountRecoveryTokenFormatter = userAccountRecoveryTokenFormatter;
        }

        public async Task<ValidationQueryResult> ExecuteAsync(ValidateUserAccountRecoveryRequestQuery query, IExecutionContext executionContext)
        {
            var tokenParts = _userAccountRecoveryTokenFormatter.Parse(query.Token);

            if (tokenParts == null)
            {
                return NotFoundResult();
            }

            var request = await _dbContext
                .UserAccountRecoveryRequests
                .AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.UserAccountRecoveryRequestId == tokenParts.UserAccountRecoveryRequestId)
                .SingleOrDefaultAsync();

            var result = ValidatePasswordRequest(request, tokenParts, query, executionContext);

            return result;
        }

        private ValidationQueryResult ValidatePasswordRequest(
            UserAccountRecoveryRequest request, 
            UserAccountRecoveryTokenParts tokenParts,
            ValidateUserAccountRecoveryRequestQuery query, 
            IExecutionContext executionContext
            )
        {
            if (request == null || !ConstantEquals(request.AuthorizationCode, tokenParts.AuthorizationCode))
            {
                return NotFoundResult();
            }

            if (request.User.UserAreaCode != query.UserAreaCode)
            {
                throw new InvalidAccountRecoveryRequestException(query, "Request received through an invalid route (incorrect user area)");
            }

            if (request.User.IsDeleted || request.User.IsSystemAccount)
            {
                throw new InvalidAccountRecoveryRequestException(query, "User not permitted to change password");
            }

            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(request.User.UserAreaCode);

            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidAccountRecoveryRequestException(query, "Cannot update the password to account in a user area that does not allow password logins.");
            }

            if (request.CompletedDate.HasValue)
            {
                return new ValidationQueryResult(new ValidationError()
                {
                    ErrorCode = AccountRecoveryErrorCodes.RequestValidation.AlreadyComplete,
                    Message = "The account recovery request has already been completed."
                });
            }

            if (request.InvalidatedDate.HasValue)
            {
                return new ValidationQueryResult(new ValidationError()
                {
                    ErrorCode = AccountRecoveryErrorCodes.RequestValidation.Invalidated,
                    Message = "The account recovery request is no longer valid."
                });
            }

            if (!IsPasswordRecoveryDateValid(userArea, request.CreateDate, executionContext))
            {
                return new ValidationQueryResult(new ValidationError()
                {
                    ErrorCode = AccountRecoveryErrorCodes.RequestValidation.Expired,
                    Message = "The account recovery request has expired."
                });
            }

            return ValidationQueryResult.ValidResult();
        }

        private static ValidationQueryResult NotFoundResult()
        {
            return new ValidationQueryResult(new ValidationError()
            {
                ErrorCode = AccountRecoveryErrorCodes.RequestValidation.NotFound,
                Message = "The account recovery request is not valid."
            });
        }

        private bool ConstantEquals(string a, string b)
        {
            var bytesA = Encoding.UTF8.GetBytes(a);
            var bytesB = Encoding.UTF8.GetBytes(b);

            return CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
        }

        private bool IsPasswordRecoveryDateValid(IUserAreaDefinition userArea, DateTime dt, IExecutionContext executionContext)
        {
            var options = _userAreaDefinitionRepository.GetOptionsByCode(userArea.UserAreaCode);
            // Non-positive indicates expiry disabled
            if (options.AccountRecovery.ValidityPeriod < TimeSpan.Zero) return true;

            return dt > executionContext.ExecutionDate.Add(-options.AccountRecovery.ValidityPeriod);
        }
    }
}