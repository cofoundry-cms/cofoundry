using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Determines if an email-based account recovery request is valid. The result is returned as a 
    /// <see cref="ValidationQueryResult"/> which describes any errors that have occured.
    /// </summary>
    public class ValidateUserAccountVerificationByEmailQueryHandler
        : IQueryHandler<ValidateUserAccountVerificationByEmailQuery, AuthorizedTaskTokenValidationResult>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public ValidateUserAccountVerificationByEmailQueryHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        public async Task<AuthorizedTaskTokenValidationResult> ExecuteAsync(ValidateUserAccountVerificationByEmailQuery query, IExecutionContext executionContext)
        {
            var options = GetOptions(query);

            var tokenResult = await _domainRepository
                .WithContext(executionContext)
                .ExecuteQueryAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    AuthorizedTaskTypeCode = UserAccountVerificationAuthorizedTaskType.Code,
                    Token = query.Token,
                });

            await RunAdditionalValidationAsync(query, tokenResult);

            var result = MapResult(tokenResult);

            return result;
        }

        private AccountVerificationOptions GetOptions(ValidateUserAccountVerificationByEmailQuery query)
        {
            var options = _userAreaDefinitionRepository.GetOptionsByCode(query.UserAreaCode).AccountVerification;

            return options;
        }

        private async Task RunAdditionalValidationAsync(ValidateUserAccountVerificationByEmailQuery query, AuthorizedTaskTokenValidationResult tokenResult)
        {
            if (tokenResult == null)
            {
                throw new InvalidUserAccountVerificationRequestException(query, $"{nameof(ValidateAuthorizedTaskTokenQuery)} should never return null.");
            }

            if (!tokenResult.IsSuccess) return;

            if (tokenResult.Data.UserAreaCode != query.UserAreaCode)
            {
                throw new InvalidUserAccountVerificationRequestException(query, "Request received through an invalid route (incorrect user area)");
            }

            var email = await _dbContext
                .Users
                .AsNoTracking()
                .FilterById(tokenResult.Data.UserId)
                .Select(u => u.Email)
                .SingleOrDefaultAsync();

            if (email != tokenResult.Data.TaskData)
            {
                tokenResult.UpdateError(UserValidationErrors.AccountVerification.RequestValidation.EmailMismatch.Create());
            }
        }

        private AuthorizedTaskTokenValidationResult MapResult(AuthorizedTaskTokenValidationResult tokenResult)
        {
            if (tokenResult.IsSuccess)
            {
                return tokenResult;
            }

            // Map the generic errors to more specific account verification errors
            var mappedError = UserValidationErrors.AccountVerification.RequestValidation.Map(tokenResult);
            return new AuthorizedTaskTokenValidationResult(mappedError);
        }
    }
}