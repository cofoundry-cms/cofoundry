using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Json
{
    /// <summary>
    /// Gets the default JsonSerializerSettings used by Cofoundry and assigned
    /// to the default Json serializer in asp.net MVC and web api.
    /// </summary>
    public interface IJsonSerializerSettingsFactory
    {
        JsonSerializerSettings Create();
    }
}
