using Cofoundry.Core.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <summary>
    /// Handles serialization for unstructured data stored in the db, e.g.
    /// Page Module data
    /// </summary>
    public class DbUnstructuredDataSerializer : IDbUnstructuredDataSerializer
    {
        #region constructor

        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;
        private readonly ILogger<DbUnstructuredDataSerializer> _logger;

        public DbUnstructuredDataSerializer(
            ILogger<DbUnstructuredDataSerializer> logger,
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory
            )
        {
            _logger = logger;
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
        }

        #endregion

        #region public methods

        public object Deserialize(string serialized, Type type)
        {
            if (String.IsNullOrEmpty(serialized))
            {
                if (type.GetTypeInfo().IsValueType) return Activator.CreateInstance(type);
                return null;
            }

            return JsonConvert.DeserializeObject(serialized, type, GetDeserializerSettings());
        }

        public T Deserialize<T>(string serialized)
        {
            if (String.IsNullOrEmpty(serialized)) return default(T);

            return JsonConvert.DeserializeObject<T>(serialized, GetDeserializerSettings());
        }

        public string Serialize(object toSerialize)
        {
            var s = JsonConvert.SerializeObject(toSerialize);

            return s;
        }

        #endregion

        #region private helpers

        private JsonSerializerSettings GetDeserializerSettings()
        {
            var settings = _jsonSerializerSettingsFactory.Create();
            settings.Error = HandleDeserializationError;

            return settings;
        }

        private void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            if (Debugger.IsAttached)
            {
                Debug.Assert(false, errorArgs.ErrorContext.Error.Message);
            }
            else
            {
                _logger.LogWarning(0, errorArgs.ErrorContext.Error, errorArgs.ErrorContext.Error.Message);
            }
            errorArgs.ErrorContext.Handled = true;
        }

        #endregion
    }
}
