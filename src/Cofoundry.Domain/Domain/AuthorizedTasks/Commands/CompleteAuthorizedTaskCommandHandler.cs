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
    /// Marks an authorized task as complete. The command does not validate 
    /// the authorization task or token, which is expected to be done prior 
    /// to invoking this command. To validate an auhtorized task token use
    /// <see cref="ValidateAuthorizedTaskTokenQuery"/>.
    /// </summary>
    public class CompleteAuthorizedTaskCommandHandler
        : ICommandHandler<CompleteAuthorizedTaskCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;

        public CompleteAuthorizedTaskCommandHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteAsync(CompleteAuthorizedTaskCommand command, IExecutionContext executionContext)
        {
            var request = await _dbContext
                .AuthorizedTasks
                .Where(t => t.AuthorizedTaskId == command.AuthorizedTaskId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(request, command.AuthorizedTaskId);

            // Complete already, no action
            if (request.CompletedDate.HasValue) return;

            if (request.InvalidatedDate.HasValue)
            {
                throw new InvalidOperationException("Cannot complete a task that has been marked as invalid.");
            }

            request.CompletedDate = executionContext.ExecutionDate;

            await _dbContext.SaveChangesAsync();
        }
    }
}