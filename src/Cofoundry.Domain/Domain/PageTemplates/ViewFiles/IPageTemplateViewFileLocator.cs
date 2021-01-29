using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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


        /// <summary>
        /// Gets the virtual path of a partial view referenced from inside a 
        /// page template, returning null if it does not exist.
        /// </summary>
        /// <param name="partialName">
        /// The name or full virtual path of the view file. If the full virtual
        /// path is already specified and exists then that path is returned
        /// </param>
        string ResolvePageTemplatePartialViewPath(string partialName);
    }
}
