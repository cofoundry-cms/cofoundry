using Cofoundry.Core.Json;
using Cofoundry.Domain.Internal;
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
        private readonly DynamicDataModelJsonSerializerSettingsCache _dynamicDataModelJsonSerializerSettingsCache;

        public DbUnstructuredDataSerializer(
            DynamicDataModelJsonSerializerSettingsCache dynamicDataModelJsonSerializerSettingsCache
            )
        {
            _dynamicDataModelJsonSerializerSettingsCache = dynamicDataModelJsonSerializerSettingsCache;
        }

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

        private JsonSerializerSettings GetDeserializerSettings()
        {
            return _dynamicDataModelJsonSerializerSettingsCache.GetInstance();
        }
    }
}
