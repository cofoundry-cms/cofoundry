using Cofoundry.Domain.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Cofoundry.Domain.Internal;

public class SettingCommandHelper
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
    private readonly EntityAuditHelper _entityAuditHelper;
    private readonly IQueryExecutor _queryExecutor;

    public SettingCommandHelper(
        CofoundryDbContext dbContext,
        IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
        EntityAuditHelper entityAuditHelper,
        IQueryExecutor queryExecutor
        )
    {
        _dbContext = dbContext;
        _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
        _entityAuditHelper = entityAuditHelper;
        _queryExecutor = queryExecutor;
    }

    public void SetSettingProperty<TCommand, TCommandProperty>(
        TCommand source,
        Expression<Func<TCommand, TCommandProperty>> propertyLambda,
        List<Setting> allSettings,
        IExecutionContext executionContext)
    {
        Type type = typeof(TCommand);

        var member = propertyLambda.Body as MemberExpression;
        var propInfo = member.Member as PropertyInfo;

        var setting = allSettings.SingleOrDefault(s => s.SettingKey == propInfo.Name);

        // Create the setting if it does not exist
        if (setting == null)
        {
            setting = new Setting();
            setting.SettingKey = propInfo.Name;
            setting.CreateDate = executionContext.ExecutionDate;
            setting.UpdateDate = executionContext.ExecutionDate;
            _dbContext.Settings.Add(setting);
        }

        var value = propInfo.GetValue(source);
        var serializedValue = _dbUnstructuredDataSerializer.Serialize(propInfo.GetValue(source));

        if (setting.SettingValue != serializedValue)
        {
            setting.SettingValue = _dbUnstructuredDataSerializer.Serialize(propInfo.GetValue(source));
            setting.UpdateDate = executionContext.ExecutionDate;
        }
    }
}