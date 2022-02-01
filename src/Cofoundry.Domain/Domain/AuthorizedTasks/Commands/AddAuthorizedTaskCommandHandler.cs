using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Adds a new authorized task record, generating a new authorization token
    /// that can be used to re-validate the task at a later date.
    /// </summary>
    public class AddAuthorizedTaskCommandHandler
        : ICommandHandler<AddAuthorizedTaskCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IAuthorizedTaskAuthorizationCodeGenerator _authorizedTaskAuthorizationCodeGenerator;
        private readonly IAuthorizedTaskTokenFormatter _authorizedTaskTokenFormatter;
        private readonly IAuthorizedTaskTypeDefinitionRepository _authorizedTaskTypeDefinitionRepository;

        public AddAuthorizedTaskCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IAuthorizedTaskAuthorizationCodeGenerator authorizedTaskAuthorizationCodeGenerator,
            IAuthorizedTaskTokenFormatter authorizedTaskTokenFormatter,
            IAuthorizedTaskTypeDefinitionRepository authorizedTaskTypeDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _authorizedTaskAuthorizationCodeGenerator = authorizedTaskAuthorizationCodeGenerator;
            _authorizedTaskTokenFormatter = authorizedTaskTokenFormatter;
            _authorizedTaskTypeDefinitionRepository = authorizedTaskTypeDefinitionRepository;
        }

        public async Task ExecuteAsync(AddAuthorizedTaskCommand command, IExecutionContext executionContext)
        {
            var taskTypeDefinition = _authorizedTaskTypeDefinitionRepository.GetRequiredByCode(command.AuthorizedTaskTypeCode);
            var ipAddressId = await GetCurrentIPAddressIdAsync(executionContext);
            await ValidateRateLimitAsync(ipAddressId, command, taskTypeDefinition, executionContext);
            var user = await GetUserAsync(command);

            var authorizedTask = MapAuthorizedTask(command, taskTypeDefinition, user, ipAddressId, executionContext);
            _dbContext.AuthorizedTasks.Add(authorizedTask);
            await _dbContext.SaveChangesAsync();

            command.OutputAuthorizedTaskId = authorizedTask.AuthorizedTaskId;
            command.OutputToken = _authorizedTaskTokenFormatter.Format(new AuthorizedTaskTokenParts()
            {
                AuthorizedTaskId = authorizedTask.AuthorizedTaskId,
                AuthorizationCode = authorizedTask.AuthorizationCode
            });
        }

        private async Task<long?> GetCurrentIPAddressIdAsync(IExecutionContext executionContext)
        {
            var command = new AddCurrentIPAddressIfNotExistsCommand();
            await _domainRepository
                .WithContext(executionContext)
                .ExecuteCommandAsync(command);

            return command.OutputIPAddressId;
        }

        private async Task ValidateRateLimitAsync(
            long? ipAddressId,
            AddAuthorizedTaskCommand command,
            IAuthorizedTaskTypeDefinition authorizedTaskTypeDefinition,
            IExecutionContext executionContext
            )
        {
            // Rate limiting may not be enabled or ip may be null if IP logging is completely disabled
            if (!ipAddressId.HasValue
                || !command.RateLimitWindow.HasValue
                || !command.RateLimitQuantity.HasValue
                || command.RateLimitWindow <= TimeSpan.Zero
                || command.RateLimitQuantity < 1
                )
            {
                return;
            }

            var dbQuery = _dbContext
                .AuthorizedTasks
                .Where(t => t.AuthorizedTaskTypeCode == authorizedTaskTypeDefinition.AuthorizedTaskTypeCode
                    && t.IPAddressId == ipAddressId.Value
                    && t.CreateDate <= executionContext.ExecutionDate
                    );

            if (command.RateLimitWindow.HasValue || command.RateLimitWindow > TimeSpan.Zero)
            {
                var dateToDetectAttempts = executionContext.ExecutionDate.Add(-command.RateLimitWindow.Value);

                dbQuery = dbQuery.Where(t => t.CreateDate > dateToDetectAttempts);
            }

            var numTasks = await dbQuery.CountAsync();

            if (numTasks >= command.RateLimitQuantity.Value)
            {
                AuthorizedTaskValidationErrors.Create.RateLimitExceeded.Throw();
            }
        }

        private Task<User> GetUserAsync(AddAuthorizedTaskCommand command)
        {
            var user = _dbContext
                .Users
                .FilterCanLogIn()
                .FilterById(command.UserId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            return user;
        }

        private AuthorizedTask MapAuthorizedTask(
            AddAuthorizedTaskCommand command,
            IAuthorizedTaskTypeDefinition authorizedTaskTypeDefinition,
            User user,
            long? ipAddressId,
            IExecutionContext executionContext
            )
        {
            string token = _authorizedTaskAuthorizationCodeGenerator.Generate();

            var authorizedTask = new AuthorizedTask()
            {
                User = user,
                AuthorizedTaskId = Guid.NewGuid(),
                CreateDate = executionContext.ExecutionDate,
                IPAddressId = ipAddressId,
                AuthorizationCode = token,
                AuthorizedTaskTypeCode = authorizedTaskTypeDefinition.AuthorizedTaskTypeCode,
                TaskData = command.TaskData
            };

            if (command.ExpireAfter > TimeSpan.Zero)
            {
                authorizedTask.ExpiryDate = executionContext.ExecutionDate.Add(command.ExpireAfter.Value);
            }

            return authorizedTask;
        }
    }
}