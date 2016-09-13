using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Allows searching for page template view files in the website directory.
    /// </summary>
    /// <remarks>
    /// Used to solve a specific problem with detecting Page Template files, but it's
    /// role could be expanded if need be.
    /// </remarks>
    public interface IPageTemplateViewFileLocator
    {
        /// <summary>
        /// Searches for page template files in the website directory containing the specified
        /// search string. Layout files should conform to the convention being located in a view folder
        /// with the name 'PageTemplates'.
        /// </summary>
        /// <param name="searchText">Optional search string to filter results.</param>
        IEnumerable<PageTemplateFile> GetPageTemplateFiles(string searchText = null);

        /// <summary>
        /// Attempts to read a view file to a string, returning null if the file does not exist.
        /// </summary>
        /// <param name="path">The virtual path to the view file.</param>
        /// <returns></returns>
        Task<string> ReadViewFileAsync(string path);

        string ResolvePageTemplatePartialViewPath(string partialName);

    }
}
