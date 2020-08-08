using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Allows searching for view files in the website directory.
    /// </summary>
    public class PageTemplateViewFileLocator : IPageTemplateViewFileLocator
    {
        private static char[] TEMPLATE_NAME_CHAR_TO_TRIM = new char[] { '_', '-' };
        const string VIEW_FILE_EXTENSION = ".cshtml";

        #region constructor

        private readonly IResourceLocator _resourceLocator;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly IEmptyActionContextFactory _emptyActionContextFactory;
        private readonly IEnumerable<IPageTemplateViewLocationRegistration> _pageTemplateViewLocationRegistrations;

        public PageTemplateViewFileLocator(
            IRazorViewEngine razorViewEngine,
            IResourceLocator resourceLocator,
            IEmptyActionContextFactory emptyActionContextFactory,
            IEnumerable<IPageTemplateViewLocationRegistration> pageTemplateViewLocationRegistrations
            )
        {
            _razorViewEngine = razorViewEngine;
            _resourceLocator = resourceLocator;
            _emptyActionContextFactory = emptyActionContextFactory;
            _pageTemplateViewLocationRegistrations = pageTemplateViewLocationRegistrations;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Searches for page layout files in the website directory containing the specified
        /// search string. Layout files should conform to the convention being located in a view folder
        /// with the name 'layouts'.
        /// </summary>
        /// <param name="searchText">Optional search string to filter results.</param>
        public IEnumerable<PageTemplateFile> GetPageTemplateFiles(string searchText = null)
        {
            return GetUnorderedPageTemplateFiles(searchText).OrderBy(l => l.FileName);
        }

        /// <summary>
        /// Gets the virtual path of a partial view referenced from inside a 
        /// page template, returning null if it does not exist.
        /// </summary>
        /// <param name="partialName">
        /// The name or full virtual path of the view file. If the full virtual
        /// path is already specified and exists then that path is returned
        /// </param>
        public string ResolvePageTemplatePartialViewPath(string partialName)
        {
            if (FileExists(partialName))
            {
                return partialName;
            }

            var view = _razorViewEngine.FindView(_emptyActionContextFactory.Create(), partialName, false);

            if (view.Success)
            {
                return view.View.Path;
            }

            return null;
        }

        #endregion

        #region private helpers

        private bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            if (path[0] != '~' && path[0] != '/') return false;

            return _resourceLocator.FileExists(path);
        }

        private IEnumerable<PageTemplateFile> GetUnorderedPageTemplateFiles(string searchText)
        {
            var templateDirecotryPaths = _pageTemplateViewLocationRegistrations.SelectMany(r => r.GetPathPrefixes());
            var templateDirectories = templateDirecotryPaths
                .Select(p => _resourceLocator.GetDirectory(p))
                .Where(d => d.Exists);

            foreach (var templateDirectoryPath in templateDirecotryPaths)
            {
                foreach (var layoutFile in SearchDirectoryForPageTemplateFiles(templateDirectoryPath, searchText))
                {
                    yield return layoutFile;
                }
            }
        }

        private IEnumerable<PageTemplateFile> SearchDirectoryForPageTemplateFiles(string directoryPath, string searchText)
        {
            var directoryContents = _resourceLocator.GetDirectory(directoryPath);

            foreach (var file in directoryContents.Where(f => !f.IsDirectory))
            {
                // filename contains the search text and is located in a 'PageTemplates' folder, but not a 'partials' folder and has the extension .cshtml
                if (Contains(file.Name, searchText)
                    && !Contains(file.Name, "_ViewStart")
                    && !Contains(file.Name, "_ViewImports")
                    && file.Name.EndsWith(VIEW_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    yield return MapPageTemplateFile(directoryPath, file);
                }
            }
            
            foreach (var childDirectoryName in directoryContents
                .Where(f => f.IsDirectory)
                .Select(f => f.Name)
                )
            {
                var childDirectoryPath = FilePathHelper.CombineVirtualPath(directoryPath, childDirectoryName);
                foreach (var file in SearchDirectoryForPageTemplateFiles(childDirectoryPath, searchText))
                {
                    yield return file;
                }
            }
        }
        
        private PageTemplateFile MapPageTemplateFile(string virtualDirectoryPath, IFileInfo file)
        {
            var fileName = Path.ChangeExtension(file.Name, null).TrimStart(TEMPLATE_NAME_CHAR_TO_TRIM);
            var virtualPath = FilePathHelper.CombineVirtualPath(virtualDirectoryPath, file.Name);
            var templateFile = new PageTemplateFile()
            {
                FileName = fileName,
                VirtualPath = virtualPath
            };

            return templateFile;
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
