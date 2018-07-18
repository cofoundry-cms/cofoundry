using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Cofoundry.Web.Framework.Mvc.Localization
{
    /// <summary>
    /// Json String localizer
    /// Used to read JSON File and add it to cache ( default 30 minutes )
    /// </summary>
    public class JsonStringLocalizer : IStringLocalizer
    {
        private List<JsonLocalizationResource> _localization = new List<JsonLocalizationResource>();
        private readonly IHostingEnvironment _env;
        private readonly IOptions<JsonLocalizationOptions> _localizationOptions;
        private readonly string _resourcesRelativePath;
        private readonly FileSystemWatcher _fileSystemWatcher;
        private readonly object _syncObject = new object();

        /// <summary>
        /// Initializes a new <see cref="JsonStringLocalizer"/>
        /// </summary>
        /// <param name="env">The <see cref="IHostingEnvironment" />.</param>
        /// <param name="localizationOptions">The <see cref="IOptions{JsonLocalizationOptions}" />.</param>
        public JsonStringLocalizer(IHostingEnvironment env, 
            IOptions<JsonLocalizationOptions> localizationOptions)
        {
            _env = env;
            _localizationOptions = localizationOptions;
            _resourcesRelativePath = localizationOptions.Value.ResourcesPath;
            _fileSystemWatcher = new FileSystemWatcher(GetJsonRelativePath());
            _fileSystemWatcher.Changed += WatchJsonFiles;
            _fileSystemWatcher.Created += WatchJsonFiles;
            _fileSystemWatcher.Deleted += WatchJsonFiles;
            InitJsonStringLocalizer(false);
        }

        /// <summary>Gets the string resource with the given name.</summary>
        /// <param name="name">The name of the string resource.</param>
        /// <returns>The string resource as a <see cref="T:Microsoft.Extensions.Localization.LocalizedString" />.</returns>
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? GetString(name, _localizationOptions.Value.DefaultCulture), value == null);
            }
        }

        /// <summary>
        /// Gets the string resource with the given name and formatted with the supplied arguments.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="arguments">The values to format the string with.</param>
        /// <returns>The formatted string resource as a <see cref="T:Microsoft.Extensions.Localization.LocalizedString" />.</returns>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? GetString(name, _localizationOptions.Value.DefaultCulture), arguments);
                return new LocalizedString(name, value, format == null);
            }
        }

        /// <summary>Gets all string resources.</summary>
        /// <param name="includeParentCultures">
        /// A <see cref="T:System.Boolean" /> indicating whether to include strings from parent cultures.
        /// </param>
        /// <returns>The strings.</returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localization.Where(l => l.Values.Keys.Any(lv => lv == CultureInfo.CurrentCulture.Name)).Select(l => new LocalizedString(l.Key, l.Values[CultureInfo.CurrentCulture.Name], true));
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Localization.IStringLocalizer" /> for a specific <see cref="T:System.Globalization.CultureInfo" />.
        /// </summary>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use.</param>
        /// <returns>A culture-specific <see cref="T:Microsoft.Extensions.Localization.IStringLocalizer" />.</returns>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer(_env, _localizationOptions);
        }

        private void InitJsonStringLocalizer(bool clearCache)
        {
            var jsonPath = GetJsonRelativePath();

            if (!clearCache)
            {
                return;
            }

            ConstructLocalizationObject(jsonPath, clearCache);
        }

        private void ConstructLocalizationObject(string jsonPath, bool clearCache)
        {
            if (_localization != null && !clearCache)
            {
                return;    
            }

            lock (_syncObject)
            {
                _localization = new List<JsonLocalizationResource>();

                foreach (var file in Directory.GetFiles(jsonPath, "*.json", SearchOption.AllDirectories))
                {
                    _localization.AddRange(JsonConvert.DeserializeObject<List<JsonLocalizationResource>>(File.ReadAllText(file)));
                }

                foreach (var assembly in _localizationOptions.Value.Assemblies)
                {
                    var path = _resourcesRelativePath.Replace("/", ".");
                    var folderName = $"{assembly.GetName().Name}.{path}";
                    foreach (var file in assembly.GetManifestResourceNames()
                        .Where(r => r.StartsWith(folderName, StringComparison.CurrentCultureIgnoreCase) && r.EndsWith(".json")))
                    {
                        var resourceStream = assembly.GetManifestResourceStream(file);
                        if (resourceStream == null)
                        {
                            continue;
                        }
                        using (var reader = new StreamReader(resourceStream))
                        {
                            _localization.AddRange(JsonConvert.DeserializeObject<List<JsonLocalizationResource>>(reader.ReadToEnd()));
                        }
                    }
                }
                MergeValues();
            }
        }

        private void MergeValues()
        {
            var groups = _localization.GroupBy(g => g.Key);

            var tempLocalization = new List<JsonLocalizationResource>();

            foreach (var group in groups)
            {
                try
                {
                    var jsonValues = group
                        .Select(s => s.Values)
                        .SelectMany(dict => dict)
                        .ToDictionary(t => t.Key, t => t.Value);

                    tempLocalization.Add(new JsonLocalizationResource()
                    {
                        Key = group.Key,
                        Values = jsonValues
                    });
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"{group.Key} could not contains two translation for the same language code", e);
                }

            }

            _localization = tempLocalization;
        }

        private string GetJsonRelativePath()
        {
            return !string.IsNullOrEmpty(_resourcesRelativePath) ? $"{GetBuildPath()}/{_resourcesRelativePath}/" : $"{_env.ContentRootPath}/Resources/";
        }

        private string GetBuildPath()
        {
            return AppContext.BaseDirectory;
        }
        
        private string GetString(string name)
        {
            return GetValueString(name, CultureInfo.CurrentCulture);
        }

        private string GetString(string name, CultureInfo cultureInfo)
        {
            return GetValueString(name, cultureInfo);
        }

        private string GetValueString(string name, CultureInfo cultureInfo)
        {
            var query = _localization.Where(l => l.Values.Keys.Any(lv => lv == cultureInfo.Name));
            var value = query.FirstOrDefault(l => l.Key == name);
            if (value == null && cultureInfo.Name == _localizationOptions.Value.DefaultCulture.Name)
            {
                return name;
            }

            return value?.Values[cultureInfo.Name];
        }

        private void WatchJsonFiles(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Deleted 
                && e.ChangeType != WatcherChangeTypes.Created
                && e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            InitJsonStringLocalizer(true);
        }
    }
}