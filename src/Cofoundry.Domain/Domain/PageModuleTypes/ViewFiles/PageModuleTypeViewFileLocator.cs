using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Allows searching for view files in the website directory.
    /// </summary>
    public class PageModuleTypeViewFileLocator : IPageModuleTypeViewFileLocator
    {
        const string FILE_EXTENSION = ".cshtml";
        const string TEMPLATES_FOLDER_NAME = "Templates";
        const string PARTIALS_FOLDER_NAME = "Partials";

        #region constructor

        private readonly IResourceLocator _resourceLocator;
        private readonly IEnumerable<IPageModuleViewLocationRegistration> _pageModuleViewLocationRegistrations;
        private readonly IPageModuleTypeCache _pageModuleTypeCache;

        public PageModuleTypeViewFileLocator(
            IResourceLocator resourceLocator,
            IEnumerable<IPageModuleViewLocationRegistration> pageModuleViewLocationRegistrations,
            IPageModuleTypeCache pageModuleTypeCache
            )
        {
            _resourceLocator = resourceLocator;
            _pageModuleViewLocationRegistrations = pageModuleViewLocationRegistrations;
            _pageModuleTypeCache = pageModuleTypeCache;
        }

        #endregion

        #region public methods
        
        /// <summary>
        /// Returns the full path of a page module view file using the file name, or
        /// null if it could not be found.
        /// </summary>
        /// <param name="pageModuleFileName">The file name (without extension) of the module. E.g 'RawHtml', 'SingleLineText'</param>
        public string GetPathByFileName(string pageModuleFileName)
        {
            var moduleLocation = GetLocationByFileName(pageModuleFileName);

            return moduleLocation?.Path;
        }

        /// <summary>
        /// Gets the virtual path for a page module template using the
        /// specified file name
        /// </summary>
        /// <param name="pageModuleFileName">The file name of the page module this template is for e.g. 'SingleLineText'</param>
        /// <param name="templateFileName">The file name of the template e.g. 'H1'</param>
        public string GetTemplatePathByTemplateFileName(string pageModuleFileName, string templateFileName)
        {
            var moduleLocation = GetLocationByFileName(pageModuleFileName);

            if (moduleLocation == null) return null;

            return moduleLocation.Templates.GetOrDefault(templateFileName)?.Path;
        }

        /// <summary>
        /// Gets a collection of paths to template files for the specified page module type.
        /// </summary>
        /// <param name="pageModuleFileName">The file name of the page module e.g. 'SingleLineText'</param>
        public IEnumerable<string> GetAllTemplatePathByModuleFileName(string pageModuleFileName)
        {
            var moduleLocation = GetLocationByFileName(pageModuleFileName);
            if (moduleLocation == null) return Enumerable.Empty<string>();

            return moduleLocation.Templates.Select(t => t.Value.Path);
        }

        #endregion

        #region private helpers

        private PageModuleTypeFileLocation GetLocationByFileName(string fileName)
        {
            var moduleLocation = GetModuleTypeFileLocationsFromCache().GetOrDefault(FormatCacheKey(fileName));
            return moduleLocation;
        }

        private string FormatCacheKey(string fileName)
        {
            // Make this case insensitive by converting to lowercase
            return fileName.ToLowerInvariant();
        }

        private Dictionary<string, PageModuleTypeFileLocation> GetModuleTypeFileLocationsFromCache()
        {
            var allModuleTypes = _pageModuleTypeCache.GetOrAddFileLocations(GetModuleTypeFileLocations);
            return allModuleTypes;
        }

        private Dictionary<string, PageModuleTypeFileLocation> GetModuleTypeFileLocations()
        {
            var viewDirectoryPaths = _pageModuleViewLocationRegistrations
                .SelectMany(r => r.GetPathPrefixes())
                .Select(p => FormatViewFolder(p))
                ;
            
            var templateFiles = new Dictionary<string, PageModuleTypeFileLocation>();
            var foldersToExclude = new string[] { TEMPLATES_FOLDER_NAME, PARTIALS_FOLDER_NAME };

            foreach (var directoryPath in viewDirectoryPaths)
            {
                var directory = _resourceLocator.GetDirectory(directoryPath);
                if (!directory.Exists) continue;

                foreach (var viewFile in FilterViewFiles(directory))
                {
                    AddTemplateToDictionary(templateFiles, directoryPath, viewFile);
                }

                foreach (var childDirectory in directory
                    .Where(d => d.IsDirectory
                        && !foldersToExclude.Any(f => d.Name.Equals(f, StringComparison.OrdinalIgnoreCase))))
                {
                    var childDirectoryPath = FilePathHelper.CombineVirtualPath(directoryPath, childDirectory.Name);

                    foreach (var viewFile in FilterChildDirectoryFiles(childDirectoryPath, directory, foldersToExclude))
                    {
                        AddTemplateToDictionary(templateFiles, childDirectoryPath, viewFile);
                    }
                }
            }

            return templateFiles;
        }

        private void AddTemplateToDictionary(Dictionary<string, PageModuleTypeFileLocation> templateFiles, string directoryPath, IFileInfo viewFile)
        {
            var templateLocation = CreateTemplateFile(directoryPath, viewFile);
            var key = FormatCacheKey(templateLocation.FileName);
            bool isUnique = !templateFiles.ContainsKey(key);
            Debug.Assert(isUnique, $"Duplicate page module type template file '{ templateLocation.FileName }' at location '{ templateLocation.Path }'");

            if (isUnique)
            {
                templateFiles.Add(key, templateLocation);
            }
        }

        private IEnumerable<IFileInfo> FilterChildDirectoryFiles(string childDirectorPath, IDirectoryContents baseDirectory, string[] foldersToExclude)
        {
            var childDirectory = _resourceLocator.GetDirectory(childDirectorPath);

            foreach (var file in FilterViewFiles(childDirectory))
            {
                yield return file;
            }
        }

        private IEnumerable<IFileInfo> FilterViewFiles(IDirectoryContents directory)
        {
            return directory
                .Where(f => !f.IsDirectory
                    && !Contains(f.Name, "_ViewStart")
                    && !Contains(f.Name, "_ViewImports")
                    && f.Name.EndsWith(FILE_EXTENSION, StringComparison.OrdinalIgnoreCase));
        }

        private PageModuleTypeFileLocation CreateTemplateFile(string directoryPath, IFileInfo file)
        {
            var templateFile = new PageModuleTypeFileLocation();

            templateFile.Path = FilePathHelper.CombineVirtualPath(directoryPath, file.Name);
            templateFile.FileName = Path.GetFileNameWithoutExtension(file.Name);

            var templatePath = FilePathHelper.CombineVirtualPath(directoryPath, TEMPLATES_FOLDER_NAME);

            var templateDirectory = _resourceLocator.GetDirectory(templatePath);
            if (templateDirectory != null)
            {
                templateFile.Templates = FilterViewFiles(templateDirectory)
                    .GroupBy(t => t.Name, (k, v) => v.FirstOrDefault()) // De-dup
                    .Select(t => new PageModuleTypeTemplateFileLocation()
                    {
                        FileName = Path.GetFileNameWithoutExtension(t.Name),
                        Path = FilePathHelper.CombineVirtualPath(templatePath, t.Name)
                    })
                    .ToDictionary(t => t.FileName);
            }
            else
            {
                templateFile.Templates = new Dictionary<string, PageModuleTypeTemplateFileLocation>();
            }

            return templateFile;
        }

        private string FormatViewFolder(string pathPrefix)
        {
            if (string.IsNullOrWhiteSpace(pathPrefix)) return null;

            var path = "/" + pathPrefix.Trim('/') + "/";

            return path;
        }

        /// <summary>
        /// Helper for readable case insensitive string comparing
        /// </summary>
        private bool Contains(string compareFrom, string compareTo)
        {
            return string.IsNullOrWhiteSpace(compareFrom) 
                || string.IsNullOrWhiteSpace(compareTo) 
                || compareFrom.IndexOf(compareTo, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion
    }
}
