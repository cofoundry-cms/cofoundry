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
        /// <summary>
        /// Creates a new JsonSerializerSettings instance .
        /// </summary>
        JsonSerializerSettings Create();

        /// <summary>
        /// Applies the json serializer settings to an existing settings instance.
        /// </summary>
        /// <param name="settings">An existing settings instance to apply updated settings to.</param>
        JsonSerializerSettings Configure(JsonSerializerSettings settings);
    }
}
