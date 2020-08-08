using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class SettingQueryHelper
    {
        #region constructor

        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;

        public SettingQueryHelper(
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer
            )
        {
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
        }

        #endregion

        public T FindSetting<T>(string key, Dictionary<string, string> allSettings)
        {
            if (!allSettings.ContainsKey(key)) return default(T);
            var setting = allSettings[key];

            if (string.IsNullOrWhiteSpace(setting)) return default(T);

            var value = _dbUnstructuredDataSerializer.Deserialize<T>(setting);

            return value;
        }

        public void SetSettingProperty<TSource, TProperty>(
            TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda,
            Dictionary<string, string> allSettings)
        {
            Type type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            var propInfo = member.Member as PropertyInfo;

            var value = FindSetting<TProperty>(propInfo.Name, allSettings);
            propInfo.SetValue(source, value);
        }
    }
}
