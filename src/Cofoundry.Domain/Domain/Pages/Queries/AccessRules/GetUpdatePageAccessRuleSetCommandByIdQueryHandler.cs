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
    public class GetUpdatePageAccessRuleSetCommandByIdQueryHandler
        : IQueryHandler<GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand>, UpdatePageAccessRuleSetCommand>
        , IPermissionRestrictedQueryHandler<GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand>, UpdatePageAccessRuleSetCommand>
    {
        private readonly CofoundryDbContext _dbContext;

        public GetUpdatePageAccessRuleSetCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<UpdatePageAccessRuleSetCommand> ExecuteAsync(GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand> query, IExecutionContext executionContext)
        {
            var dbPage = await _dbContext
                .Pages
                .AsNoTracking()
                .Include(r => r.AccessRules)
                .FilterById(query.Id)
                .SingleOrDefaultAsync();

            if (dbPage == null) return null;

            var violationAction = EnumParser.ParseOrNull<AccessRuleViolationAction>(dbPage.AccessRuleViolationActionId);
            if (!violationAction.HasValue)
            {
                throw new InvalidOperationException($"{nameof(AccessRuleViolationAction)} of value {dbPage.AccessRuleViolationActionId} could not be parsed on a page with an id of {dbPage.PageId}.");
            }

            var command = new UpdatePageAccessRuleSetCommand()
            {
                PageId = dbPage.PageId,
                UserAreaCodeForSignInRedirect = dbPage.UserAreaCodeForSignInRedirect,
                ViolationAction = violationAction.Value
            };

            command.AccessRules = dbPage
                .AccessRules
                .Select(r => new UpdatePageAccessRuleSetCommand.AddOrUpdatePageAccessRuleCommand()
                {
                    PageAccessRuleId = r.PageAccessRuleId,
                    UserAreaCode = r.UserAreaCode,
                    RoleId = r.RoleId
                })
                .ToList();

            return command;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetPatchableCommandByIdQuery<UpdatePageAccessRuleSetCommand> query)
        {
            yield return new PageReadPermission();
        }
    }
}
