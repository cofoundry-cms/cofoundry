using Cofoundry.Core.ExecutionDurationRandomizer;
using Cofoundry.Domain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Validates user credentials. If the authentication was successful then user information 
/// pertinent to sign in is returned, otherwise error information is returned detailing
/// why the authentication failed.
/// </summary>
/// <remakrs>
/// This query breaks CQS principle somewhat as it does has some security related side-effects i.e.
/// password rehashing and attempt logging, however this is necessary to encapsulate
/// the security features without pushing the implementation onto the API consumer.
/// </remakrs>
public class AuthenticateUserCredentialsQueryHandler
    : IQueryHandler<AuthenticateUserCredentialsQuery, UserCredentialsAuthenticationResult>
    , IIgnorePermissionCheckHandler
{
    private readonly UserAuthenticationHelper _userAuthenticationHelper;
    private readonly ILogger<AuthenticateUserCredentialsQueryHandler> _logger;
    private readonly CofoundryDbContext _dbContext;
    private readonly IDomainRepository _domainRepository;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly IUserDataFormatter _userDataFormatter;
    private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
    private readonly IExecutionDurationRandomizerScopeManager _executionDurationRandomizerScopeManager;

    public AuthenticateUserCredentialsQueryHandler(
        ILogger<AuthenticateUserCredentialsQueryHandler> logger,
        CofoundryDbContext dbContext,
        IDomainRepository domainRepository,
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        UserAuthenticationHelper userAuthenticationHelper,
        IUserDataFormatter userDataFormatter,
        IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
        IExecutionDurationRandomizerScopeManager executionDurationRandomizerScopeManager
        )
    {
        _userAuthenticationHelper = userAuthenticationHelper;
        _logger = logger;
        _dbContext = dbContext;
        _domainRepository = domainRepository;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _userDataFormatter = userDataFormatter;
        _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
        _executionDurationRandomizerScopeManager = executionDurationRandomizerScopeManager;
    }

    public async Task<UserCredentialsAuthenticationResult> ExecuteAsync(AuthenticateUserCredentialsQuery query, IExecutionContext executionContext)
    {
        var options = _userAreaDefinitionRepository.GetOptionsByCode(query.UserAreaCode).Authentication;
        await using (_executionDurationRandomizerScopeManager.Create(options.ExecutionDuration))
        {
            return await ExecuteInternalAsync(query, executionContext);
        }
    }

    private async Task<UserCredentialsAuthenticationResult> ExecuteInternalAsync(AuthenticateUserCredentialsQuery query, IExecutionContext executionContext)
    {
        var uniqueUsername = _userDataFormatter.UniquifyUsername(query.UserAreaCode, query.Username);
        if (string.IsNullOrWhiteSpace(uniqueUsername) || string.IsNullOrWhiteSpace(query.Password))
        {
            return GetAuthenticationFailedForUnknownUserResult(query);
        }

        var maxAttemptsValidationResult = await ValidateMaxAuthenticationAttemptsAsync(query, uniqueUsername, executionContext);
        if (maxAttemptsValidationResult != null) return maxAttemptsValidationResult;

        var dbUser = await GetUserAsync(query.UserAreaCode, uniqueUsername);
        if (dbUser == null)
        {
            var failedResult = GetAuthenticationFailedForUnknownUserResult(query);
            await LogResultAsync(failedResult, query, uniqueUsername, executionContext);

            return GetAuthenticationFailedForUnknownUserResult(query);
        }

        var passwordVerificationResult = VerifyPassword(query, dbUser);
        await RehashPasswordIfNeededAsync(query, passwordVerificationResult, dbUser);

        var user = MapUser(passwordVerificationResult, dbUser);
        var result = MapResult(user, query);
        await LogResultAsync(result, query, uniqueUsername, executionContext);

        return result;
    }

    private UserCredentialsAuthenticationResult GetAuthenticationFailedForUnknownUserResult(AuthenticateUserCredentialsQuery query)
    {
        _logger.LogDebug("Authentication failed for unknown user in user area {UserAreaCode}", query.UserAreaCode);

        var result = new UserCredentialsAuthenticationResult();
        result.Error = UserValidationErrors.Authentication.InvalidCredentials.Create(query.PropertyToValidate);

        return result;
    }

    private async Task<UserCredentialsAuthenticationResult> ValidateMaxAuthenticationAttemptsAsync(AuthenticateUserCredentialsQuery query, string uniqueUsername, IExecutionContext executionContext)
    {
        ;
        var hasExceededMaxAuthenticationAttempts = await _domainRepository
            .WithContext(executionContext)
            .ExecuteQueryAsync(new HasExceededMaxAuthenticationAttemptsQuery()
            {
                UserAreaCode = query.UserAreaCode,
                Username = uniqueUsername
            });

        if (!hasExceededMaxAuthenticationAttempts) return null;

        _logger.LogDebug("Authentication failed due to too many failed attempts {UserAreaCode}", query.UserAreaCode);

        return new UserCredentialsAuthenticationResult()
        {
            Error = UserValidationErrors.Authentication.TooManyFailedAttempts.Create(query.PropertyToValidate)
        };
    }

    private Task<User> GetUserAsync(string userAreaCode, string uniqueUsername)
    {
        return _dbContext
            .Users
            .FilterByUserArea(userAreaCode)
            .FilterCanSignIn()
            .Where(u => u.UniqueUsername == uniqueUsername)
            .FirstOrDefaultAsync();
    }

    private UserSignInInfo MapUser(PasswordVerificationResult passwordVerificationResult, User dbUser)
    {
        ArgumentNullException.ThrowIfNull(dbUser);

        if (passwordVerificationResult == PasswordVerificationResult.Failed) return null;

        var userSignInInfo = new UserSignInInfo()
        {
            RequirePasswordChange = dbUser.RequirePasswordChange,
            UserAreaCode = dbUser.UserAreaCode,
            UserId = dbUser.UserId,
            IsAccountVerified = dbUser.AccountVerifiedDate.HasValue
        };

        return userSignInInfo;
    }

    private PasswordVerificationResult VerifyPassword(AuthenticateUserCredentialsQuery query, User dbUser)
    {
        ArgumentNullException.ThrowIfNull(dbUser);
        var verificationResult = _userAuthenticationHelper.VerifyPassword(dbUser, query.Password);

        switch (verificationResult)
        {
            case PasswordVerificationResult.Failed:
                _logger.LogDebug("Authentication failed for user {UserId}", dbUser.UserId);
                break;
            case PasswordVerificationResult.SuccessRehashNeeded:
                _logger.LogDebug("Authentication success for user {UserId} (rehash needed)", dbUser.UserId);
                break;
            case PasswordVerificationResult.Success:
                _logger.LogDebug("Authentication success for user {UserId}", dbUser.UserId);
                break;
            default:
                throw new InvalidOperationException("Unrecognised PasswordVerificationResult: " + verificationResult);
        }

        return verificationResult;
    }

    private async Task RehashPasswordIfNeededAsync(AuthenticateUserCredentialsQuery query, PasswordVerificationResult passwordVerificationResult, User user)
    {
        if (passwordVerificationResult != PasswordVerificationResult.SuccessRehashNeeded) return;

        // This breaks CQS principle somewhat but we need to ensure that we rehash the 
        // password at first opportunity irrespective of how the API is used
        _logger.LogDebug("Rehashing password for user {UserId}", user.UserId);
        _passwordUpdateCommandHelper.UpdatePasswordHash(query.Password, user);

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// The user has been mapped, so complete the mapping of the outer result.
    /// </summary>
    private UserCredentialsAuthenticationResult MapResult(
        UserSignInInfo user,
        AuthenticateUserCredentialsQuery query
        )
    {
        var result = new UserCredentialsAuthenticationResult();

        if (user == null)
        {
            result.Error = UserValidationErrors.Authentication.InvalidCredentials.Create(query.PropertyToValidate);
            return result;
        }

        if (result.Error != null)
        {
            throw new InvalidOperationException($"Unexpected error state for a successful request: {result.Error.ErrorCode}");
        }

        result.User = user;
        result.IsSuccess = true;

        return result;
    }

    private async Task LogResultAsync(
        UserCredentialsAuthenticationResult result,
        AuthenticateUserCredentialsQuery query,
        string uniqueUsername,
        IExecutionContext executionContext
        )
    {
        ICommand command;

        if (result.IsSuccess)
        {
            command = new LogSuccessfulAuthenticationCommand() { UserId = result.User.UserId };
        }
        else
        {
            command = new LogFailedAuthenticationAttemptCommand(query.UserAreaCode, uniqueUsername);
        }

        await _domainRepository
            .WithContext(executionContext)
            .ExecuteCommandAsync(command);
    }
}
