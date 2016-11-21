using Cofoundry.Core;
using Cofoundry.Core.EmbeddedResources;
using Cofoundry.Domain;
using Cofoundry.Web.ModularMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Cofoundry.Web
{
    /// <summary>
    /// Allows searching for view files in the website directory.
    /// </summary>
    public class PageTemplateViewFileLocator : IPageTemplateViewFileLocator
    {
        const string FILE_EXTENSION = ".cshtml";
        const string DEFAULT_LAYOUT_NAME = "layout";
        const string PAGE_TEMPLATES_FOLDER_NAME = "PageTemplates";
        const string PARTIALS_FOLDER_NAME = "partials";
        static string[] PAGE_TEMPLATE_DIRECTORIES_TO_EXCLUDE = new string[] {
            "/admin/"
        };


        #region constructor

        private readonly IResourceLocator _resourceLocator;
        private readonly AssemblyResourceViewEngine _viewEngine;

        public PageTemplateViewFileLocator(
            AssemblyResourceViewEngine viewEngine,
            IResourceLocator resourceLocator
            )
        {
            _viewEngine = viewEngine;
            _resourceLocator = resourceLocator;
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
            return GetUnorderedPageTemplateFiles(searchText).OrderBy(l => l.Name);
        }

        public string ResolvePageTemplatePartialViewPath(string partialName)
        {
            if (FileExists(partialName))
            {
                return partialName;
            }

            // Check locations
            foreach (var location in _viewEngine
                .PartialViewLocationFormats
                .Select(f => string.Format(f, partialName, PAGE_TEMPLATES_FOLDER_NAME)))
            {
                if (FileExists(location)) return location;
            }

            return null;
        }

        #endregion

        #region private helpers

        private IEnumerable<PageTemplateFile> GetUnorderedPageTemplateFiles(string searchText)
        {
            var viewDirectories = _viewEngine
                .ViewLocationFormats
                .Where(f => (f.Contains("{1}") || f.Contains(PAGE_TEMPLATES_FOLDER_NAME)) && !PAGE_TEMPLATE_DIRECTORIES_TO_EXCLUDE.Any(e => f.StartsWith(e)))
                .Select(f => Path.ChangeExtension(string.Format(f, string.Empty, PAGE_TEMPLATES_FOLDER_NAME), null));

            foreach (var directory in viewDirectories)
            {
                foreach (var layoutFile in SearchDirectoryForPageTemplateFiles(_resourceLocator.GetDirectory(directory), searchText))
                {
                    yield return layoutFile;
                }
            }
        }

        private IEnumerable<PageTemplateFile> SearchDirectoryForPageTemplateFiles(IResourceDirectory directory, string searchText)
        {
            foreach (var file in directory.GetFiles())
            {
                // filename contains the search text and is located in a 'PageTemplates' folder, but not a 'partials' folder and has the extension .cshtml
                if (Contains(file.Name, searchText)
                    && !Contains(file.VirtualPath, PARTIALS_FOLDER_NAME)
                    && !Contains(file.VirtualPath, "_ViewStart")
                    && file.Name.EndsWith(FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    yield return MapPageTemplateFile(file);
                }
            }

            foreach (var childDirectory in directory.GetDirectories())
            foreach (var file in SearchDirectoryForPageTemplateFiles(childDirectory, searchText))
            {
                yield return file;
            }
        }

        private PageTemplateFile MapPageTemplateFile(IResourceFile file)
        {
            var name = TextFormatter.PascalCaseToSentence(Path.ChangeExtension(file.Name, null).TrimStart(new char[] { '_', '-' }));

            // if the file is just called 'layout' expand the name to the parent folder.
            if (name.Equals(DEFAULT_LAYOUT_NAME, StringComparison.OrdinalIgnoreCase))
            {
                var parts = file
                    .VirtualPath
                    .Split('/');

                if (parts.Length > 2)
                {
                    name = parts[parts.Length - 2] + " " + name;
                }
            }

            var templateFile = new PageTemplateFile()
            {
                FileName = file.Name,
                FullPath = file.VirtualPath,
                Name = name
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
