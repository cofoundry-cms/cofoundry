using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Cofoundry.Web.Framework.Mvc.Localization
{
    /// <summary>
    /// Factory the create the JsonStringLocalizer
    /// </summary>
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        readonly IHostingEnvironment _env;
        readonly IMemoryCache _memCache;
        readonly IOptions<JsonLocalizationOptions> _localizationOptions;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="env">The <see cref="IHostingEnvironment" />.</param>
        /// <param name="localizationOptions">The <see cref="IOptions{JsonLocalizationOptions}" />.</param>
        public JsonStringLocalizerFactory(
                IHostingEnvironment env,
                IOptions<JsonLocalizationOptions> localizationOptions)
        {
            _env = env;
            _localizationOptions = localizationOptions ?? throw new ArgumentNullException(nameof(localizationOptions));
        }

        /// <summary>
        /// Creates an <see cref="JsonStringLocalizer" />.
        /// </summary>
        /// <param name="resourceSource">The <see cref="T:System.Type" />.</param>
        /// <returns>The <see cref="JsonStringLocalizer" />.</returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_env, _localizationOptions);
        }

        /// <summary>
        /// Creates an <see cref="JsonStringLocalizer" />.
        /// </summary>
        /// <param name="baseName">The base name of the resource to load strings from.</param>
        /// <param name="location">The location to load resources from.</param>
        /// <returns>The <see cref="JsonStringLocalizer" />.</returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_env, _localizationOptions);
        }
    }
}