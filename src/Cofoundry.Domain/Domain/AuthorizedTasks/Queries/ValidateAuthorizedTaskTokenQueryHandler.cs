using Cofoundry.Domain.Data;
using System.Security.Cryptography;
using System.Text;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Determines if an account recovery request is valid. The result is returned as a 
/// <see cref="ValidationQueryResult"/> which describes any errors that have occurred.
/// </summary>
public class ValidateAuthorizedTaskTokenQueryHandler
    : IQueryHandler<ValidateAuthorizedTaskTokenQuery, AuthorizedTaskTokenValidationResult>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IAuthorizedTaskTokenFormatter _authorizedTaskTokenFormatter;
    private readonly IAuthorizedTaskTypeDefinitionRepository _authorizedTaskTypeDefinitionRepository;

    public ValidateAuthorizedTaskTokenQueryHandler(
        CofoundryDbContext dbContext,
        IAuthorizedTaskTokenFormatter authorizedTaskTokenFormatter,
        IAuthorizedTaskTypeDefinitionRepository authorizedTaskTypeDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _authorizedTaskTokenFormatter = authorizedTaskTokenFormatter;
        _authorizedTaskTypeDefinitionRepository = authorizedTaskTypeDefinitionRepository;
    }

    public async Task<AuthorizedTaskTokenValidationResult> ExecuteAsync(ValidateAuthorizedTaskTokenQuery query, IExecutionContext executionContext)
    {
        var authrizedTaskType = _authorizedTaskTypeDefinitionRepository.GetRequiredByCode(query.AuthorizedTaskTypeCode);
        var tokenParts = _authorizedTaskTokenFormatter.Parse(query.Token);

        if (tokenParts == null)
        {
            return NotFoundResult();
        }

        var authorizedTask = await _dbContext
            .AuthorizedTasks
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.AuthorizedTaskId == tokenParts.AuthorizedTaskId
                && r.AuthorizedTaskTypeCode == authrizedTaskType.AuthorizedTaskTypeCode)
            .SingleOrDefaultAsync();

        var result = Validate(authorizedTask, tokenParts, query, executionContext);

        if (result.IsSuccess)
        {
            result.Data = new AuthorizedTaskTokenValidationResultData()
            {
                TaskData = authorizedTask.TaskData,
                AuthorizedTaskId = authorizedTask.AuthorizedTaskId,
                UserId = authorizedTask.User.UserId,
                UserAreaCode = authorizedTask.User.UserAreaCode
            };
        }

        return result;
    }

    private AuthorizedTaskTokenValidationResult Validate(
        AuthorizedTask authorizedTask,
        AuthorizedTaskTokenParts tokenParts,
        ValidateAuthorizedTaskTokenQuery query,
        IExecutionContext executionContext
        )
    {
        if (authorizedTask == null || !ConstantEquals(authorizedTask.AuthorizationCode, tokenParts.AuthorizationCode))
        {
            return NotFoundResult();
        }

        if (authorizedTask.User.IsSystemAccount)
        {
            throw new InvalidAuthorizedTaskTokenException(query, "The system account cannot be used for authorized tasks");
        }

        if (authorizedTask.CompletedDate.HasValue)
        {
            return new AuthorizedTaskTokenValidationResult(AuthorizedTaskValidationErrors.TokenValidation.AlreadyComplete.Create());
        }

        if (authorizedTask.InvalidatedDate.HasValue || !authorizedTask.User.IsEnabled())
        {
            return new AuthorizedTaskTokenValidationResult(AuthorizedTaskValidationErrors.TokenValidation.Invalidated.Create());
        }

        if (IsExpired(authorizedTask, executionContext))
        {
            return new AuthorizedTaskTokenValidationResult(AuthorizedTaskValidationErrors.TokenValidation.Expired.Create());
        }

        return new AuthorizedTaskTokenValidationResult()
        {
            IsSuccess = true
        };
    }

    private static AuthorizedTaskTokenValidationResult NotFoundResult()
    {
        return new AuthorizedTaskTokenValidationResult(AuthorizedTaskValidationErrors.TokenValidation.NotFound.Create());
    }

    private bool ConstantEquals(string a, string b)
    {
        var bytesA = Encoding.UTF8.GetBytes(a);
        var bytesB = Encoding.UTF8.GetBytes(b);

        return CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
    }

    private bool IsExpired(AuthorizedTask task, IExecutionContext executionContext)
    {
        // Null indicates expiry disabled
        if (!task.ExpiryDate.HasValue) return false;

        return task.ExpiryDate < executionContext.ExecutionDate;
    }
}
