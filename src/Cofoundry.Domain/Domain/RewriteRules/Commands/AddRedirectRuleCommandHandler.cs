using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class AddRedirectRuleCommandHandler 
        : ICommandHandler<AddRedirectRuleCommand>
        , IPermissionRestrictedCommandHandler<AddRedirectRuleCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;

        public AddRedirectRuleCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(AddRedirectRuleCommand command, IExecutionContext executionContext)
        {
            var rule = new RewriteRule();

            rule.WriteFrom = command.WriteFrom;
            rule.WriteTo = command.WriteTo;
            _entityAuditHelper.SetCreated(rule, executionContext);

            // TODO: Should be checking uniqueness?
 
            _dbContext.RewriteRules.Add(rule);
            await _dbContext.SaveChangesAsync();
            command.OutputRedirectRuleId = rule.RewriteRuleId;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddRedirectRuleCommand command)
        {
            yield return new RewriteRuleCreatePermission();
        }

        #endregion
    }
}
