using Cofoundry.Domain.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Cofoundry.Domain.Internal;

public class SettingCommandHelper
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;

    public SettingCommandHelper(
        CofoundryDbContext dbContext,
        IDbUnstructuredDataSerializer dbUnstructuredDataSerializer
        )
    {
        _dbContext = dbContext;
        _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
    }

    public void SetSettingProperty<TCommand, TCommandProperty>(
        TCommand source,
        Expression<Func<TCommand, TCommandProperty>> propertyLambda,
        IReadOnlyCollection<Setting> allSettings,
        IExecutionContext executionContext
        )
    {
        var type = typeof(TCommand);

        var member = propertyLambda.Body as MemberExpression;
        var propInfo = member?.Member as PropertyInfo;
        if (propInfo == null)
        {
            throw new ArgumentException("Expression must reference a property", nameof(propertyLambda));
        }

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
            setting.SettingValue = _dbUnstructuredDataSerializer.Serialize(propInfo.GetValue(source)) ?? string.Empty;
            setting.UpdateDate = executionContext.ExecutionDate;
        }
    }
}