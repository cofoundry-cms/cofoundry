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
    public class GetUpdatePageAccessRuleCommandByIdQueryHandler
        : IQueryHandler<GetUpdateCommandByIdQuery<UpdatePageAccessRulesCommand>, UpdatePageAccessRulesCommand>
        , IPermissionRestrictedQueryHandler<GetUpdateCommandByIdQuery<UpdatePageAccessRulesCommand>, UpdatePageAccessRulesCommand>
    {
        private readonly CofoundryDbContext _dbContext;

        public GetUpdatePageAccessRuleCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<UpdatePageAccessRulesCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdatePageAccessRulesCommand> query, IExecutionContext executionContext)
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

            var command = new UpdatePageAccessRulesCommand()
            {
                PageId = dbPage.PageId,
                UserAreaCodeForLoginRedirect = dbPage.UserAreaCodeForLoginRedirect,
                ViolationAction = violationAction.Value
            };

            command.AccessRules = dbPage
                .AccessRules
                .Select(r => new UpdatePageAccessRulesCommand.AddOrUpdatePageAccessRuleCommand()
                {
                    PageAccessRuleId = r.PageAccessRuleId,
                    UserAreaCode = r.UserAreaCode,
                    RoleId = r.RoleId
                })
                .ToList();

            return command;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetUpdateCommandByIdQuery<UpdatePageAccessRulesCommand> query)
        {
            yield return new PageReadPermission();
        }
    }
}
