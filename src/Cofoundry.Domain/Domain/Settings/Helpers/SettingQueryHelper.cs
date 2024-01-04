using Cofoundry.Domain.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Cofoundry.Domain.Internal;

public class SettingQueryHelper
{
    private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;

    public SettingQueryHelper(
        IDbUnstructuredDataSerializer dbUnstructuredDataSerializer
        )
    {
        _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
    }

    public T? FindSetting<T>(string key, IReadOnlyDictionary<string, string> allSettings)
    {
        if (!allSettings.ContainsKey(key))
        {
            return default;
        }

        var setting = allSettings[key];

        if (string.IsNullOrWhiteSpace(setting))
        {
            return default;
        }

        var value = _dbUnstructuredDataSerializer.Deserialize<T>(setting);

        return value;
    }

    public void SetSettingProperty<TSource, TProperty>(
        TSource source,
        Expression<Func<TSource, TProperty>> propertyLambda,
        IReadOnlyDictionary<string, string> allSettings
        )
    {
        var member = propertyLambda.Body as MemberExpression;
        var propInfo = member?.Member as PropertyInfo;
        if (propInfo == null)
        {
            throw new ArgumentException("Expression must reference a property", nameof(propertyLambda));
        }

        var value = FindSetting<TProperty>(propInfo.Name, allSettings);
        propInfo.SetValue(source, value);
    }
}