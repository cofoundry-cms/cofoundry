using System.Collections.Generic;

namespace Cofoundry.Web.Framework.Mvc.Localization
{
    /// <summary>
    /// DTO for localization key value parirs
    /// </summary>
    public class JsonLocalizationResource
    {
        /// <summary>
        /// The key for translation.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The language value pairs
        /// </summary>
        public Dictionary<string, string> Values = new Dictionary<string, string>();
    }
}