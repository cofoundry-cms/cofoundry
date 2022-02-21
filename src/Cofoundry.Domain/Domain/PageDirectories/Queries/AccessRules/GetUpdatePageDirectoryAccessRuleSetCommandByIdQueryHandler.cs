using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdatePageDirectoryAccessRuleSetCommandByIdQueryHandler
        : IQueryHandler<GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>, UpdatePageDirectoryAccessRuleSetCommand>
        , IPermissionRestrictedQueryHandler<GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand>, UpdatePageDirectoryAccessRuleSetCommand>
    {
        private readonly CofoundryDbContext _dbContext;

        public GetUpdatePageDirectoryAccessRuleSetCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<UpdatePageDirectoryAccessRuleSetCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand> query, IExecutionContext executionContext)
        {
            var dbPageDirectory = await _dbContext
                .PageDirectories
                .AsNoTracking()
                .Include(r => r.AccessRules)
                .FilterById(query.Id)
                .SingleOrDefaultAsync();

            if (dbPageDirectory == null) return null;

            var violationAction = EnumParser.ParseOrNull<AccessRuleViolationAction>(dbPageDirectory.AccessRuleViolationActionId);
            if (!violationAction.HasValue)
            {
                throw new InvalidOperationException($"{nameof(AccessRuleViolationAction)} of value {dbPageDirectory.AccessRuleViolationActionId} could not be parsed on a page directory with an id of {dbPageDirectory.PageDirectoryId}.");
            }

            var command = new UpdatePageDirectoryAccessRuleSetCommand()
            {
                PageDirectoryId = dbPageDirectory.PageDirectoryId,
                UserAreaCodeForSignInRedirect = dbPageDirectory.UserAreaCodeForSignInRedirect,
                ViolationAction = violationAction.Value
            };

            command.AccessRules = dbPageDirectory
                .AccessRules
                .Select(r => new UpdatePageDirectoryAccessRuleSetCommand.AddOrUpdatePageDirectoryAccessRuleCommand()
                {
                    PageDirectoryAccessRuleId = r.PageDirectoryAccessRuleId,
                    UserAreaCode = r.UserAreaCode,
                    RoleId = r.RoleId
                })
                .ToList();

            return command;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetPatchableCommandByIdQuery<UpdatePageDirectoryAccessRuleSetCommand> query)
        {
            yield return new PageDirectoryReadPermission();
        }
    }
}
