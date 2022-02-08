using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Determines if an email-based account recovery request is valid. The result is returned as a 
    /// <see cref="ValidationQueryResult"/> which describes any errors that have occured.
    /// </summary>
    public class ValidateUserAccountRecoveryByEmailQueryHandler
        : IQueryHandler<ValidateUserAccountRecoveryByEmailQuery, AuthorizedTaskTokenValidationResult>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public ValidateUserAccountRecoveryByEmailQueryHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        public async Task<AuthorizedTaskTokenValidationResult> ExecuteAsync(ValidateUserAccountRecoveryByEmailQuery query, IExecutionContext executionContext)
        {
            var options = GetOptions(query);

            var tokenResult = await _domainRepository
                .WithContext(executionContext)
                .ExecuteQueryAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    AuthorizedTaskTypeCode = UserAccountRecoveryAuthorizedTaskType.Code,
                    Token = query.Token
                });

            RunAdditionalValidation(query, tokenResult);

            var result = MapResult(tokenResult);

            return result;
        }

        private AccountRecoveryOptions GetOptions(ValidateUserAccountRecoveryByEmailQuery query)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(query.UserAreaCode);

            if (!userArea.AllowPasswordSignIn)
            {
                throw new InvalidAccountRecoveryRequestException(query, "Cannot update the password to account in a user area that does not allow password sign in.");
            }

            if (!userArea.UseEmailAsUsername)
            {
                throw new InvalidAccountRecoveryRequestException(query, $"Cannot reset the password because the {userArea.Name} user area does not require email addresses.");
            }

            var options = _userAreaDefinitionRepository.GetOptionsByCode(userArea.UserAreaCode).AccountRecovery;

            return options;
        }

        private static void RunAdditionalValidation(ValidateUserAccountRecoveryByEmailQuery query, AuthorizedTaskTokenValidationResult tokenResult)
        {
            if (tokenResult == null)
            {
                throw new InvalidAccountRecoveryRequestException(query, $"{nameof(ValidateAuthorizedTaskTokenQuery)} should never return null.");
            }

            if (tokenResult.IsSuccess && tokenResult.Data.UserAreaCode != query.UserAreaCode)
            {
                throw new InvalidAccountRecoveryRequestException(query, "Request received through an invalid route (incorrect user area)");
            }
        }

        private AuthorizedTaskTokenValidationResult MapResult(AuthorizedTaskTokenValidationResult tokenResult)
        {
            if (tokenResult.IsSuccess)
            {
                return tokenResult;
            }

            // Map the generic errors to more specific password reset errors
            var mappedError = UserValidationErrors.AccountRecovery.RequestValidation.Map(tokenResult);
            return new AuthorizedTaskTokenValidationResult(mappedError);
        }
    }
}