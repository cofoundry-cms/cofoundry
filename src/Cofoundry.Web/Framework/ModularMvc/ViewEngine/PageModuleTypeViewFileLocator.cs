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
    public class PageModuleTypeViewFileLocator : IPageModuleTypeViewFileLocator
    {
        const string FILE_EXTENSION = ".cshtml";

        #region constructor

        private readonly IResourceLocator _resourceLocator;
        private readonly IPageModuleViewLocationRegistration[] _pageModuleViewLocationRegistrations;

        public PageModuleTypeViewFileLocator(
            IResourceLocator resourceLocator,
            IPageModuleViewLocationRegistration[] pageModuleViewLocationRegistrations
            )
        {
            _resourceLocator = resourceLocator;
            _pageModuleViewLocationRegistrations = pageModuleViewLocationRegistrations;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the virtual path for a page module template using the
        /// specified file name
        /// </summary>
        /// <param name="pageModuleFileName">The file name of the page module this template is for e.g. 'SingleLineText'</param>
        /// <param name="templateFileName">The file name of the template e.g. 'H1'</param>
        public string GetTemplatePathByTemplateFileName(string pageModuleFileName, string templateFileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of paths to template files for the specified page module type.
        /// </summary>
        /// <param name="pageModuleFileName">The file name of the page module e.g. 'SingleLineText'</param>
        public IEnumerable<string> GetAllTemplatePathByModuleFileName(string pageModuleFileName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private helpers

        private IEnumerable<PageTemplateFile> GetUnorderedViewFiles(string searchText)
        {
            var viewDirectories = _pageModuleViewLocationRegistrations
                .SelectMany(r => r.GetPathPrefixes())
                .Select(p => FormatViewFolder(p))
                .Select(p => _resourceLocator.GetDirectory(p))
                .Where(d => d != null)
                ;

            // TODO:
            // - need to loop though the directories and sub directories to look for template files
            // - Need to cache this coz we could be using the path resolver a lot
            // - How do we separate module views from template files which we will also have to cache? IPageModuleTypeCache?
            // - Since we don't need the AssemblyResourceViewEngine, we can probably move this into the Domain

            foreach (var directory in viewDirectories)
            {
                foreach (var layoutFile in SearchDirectoryForPageModuleFiles(directory))
                {
                    yield return layoutFile;
                }
            }
        }

        private string FormatViewFolder(string pathPrefix)
        {
            if (string.IsNullOrWhiteSpace(pathPrefix)) return null;

            var path = "/" + pathPrefix.Trim('/') + "/";

            return path;
        }

        private IEnumerable<PageTemplateFile> SearchDirectoryForPageModuleFiles(IResourceDirectory directory)
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
            foreach (var file in SearchDirectoryForPageModuleFiles(childDirectory, searchText))
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
