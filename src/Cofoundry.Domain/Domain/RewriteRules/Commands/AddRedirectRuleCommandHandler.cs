using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class AddRedirectRuleCommandHandler
    : ICommandHandler<AddRedirectRuleCommand>
    , IPermissionRestrictedCommandHandler<AddRedirectRuleCommand>
{
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

    public IEnumerable<IPermissionApplication> GetPermissions(AddRedirectRuleCommand command)
    {
        yield return new RewriteRuleCreatePermission();
    }
}