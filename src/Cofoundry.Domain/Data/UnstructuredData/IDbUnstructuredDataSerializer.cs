using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Handles serialization for unstructured data stored in the db, e.g.
    /// Page Module data
    /// </summary>
    public interface IDbUnstructuredDataSerializer
    {
        object Deserialize(string serialized, Type type);
        T Deserialize<T>(string serialized);
        string Serialize(object toSerialize);
    }
}
