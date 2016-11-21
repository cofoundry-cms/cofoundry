using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Allows searching for page template view files in an application
    /// </summary>
    public interface IPageTemplateViewFileLocator
    {
        /// <summary>
        /// Searches for page template files in the website directory containing the specified
        /// search string. Layout files should conform to the convention being located in a view folder
        /// with the name 'PageTemplates'.
        /// </summary>
        /// <param name="searchText">Optional search string to filter results.</param>
        IEnumerable<PageTemplateFile> GetPageTemplateFiles(string searchText = null);

        string ResolvePageTemplatePartialViewPath(string partialName);
    }
}
