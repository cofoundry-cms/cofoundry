using Cofoundry.Core.ErrorLogging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Handles serialization for unstructured data stored in the db, e.g.
    /// Page Module data
    /// </summary>
    public class DbUnstructuredDataSerializer : IDbUnstructuredDataSerializer
    {
        #region constructor

        private readonly IErrorLoggingService _errorLoggingService;

        public DbUnstructuredDataSerializer(
            IErrorLoggingService errorLoggingService
            )
        {
            _errorLoggingService = errorLoggingService;
        }

        #endregion

        #region public methods

        public object Deserialize(string serialized, Type type)
        {
            if (String.IsNullOrEmpty(serialized))
            {
                if (type.IsValueType) return Activator.CreateInstance(type);
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
            var settings = JsonConvert.DefaultSettings();
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
                _errorLoggingService.Log(new Exception(errorArgs.ErrorContext.Error.Message));
            }
            errorArgs.ErrorContext.Handled = true;
        }

        #endregion
    }
}
