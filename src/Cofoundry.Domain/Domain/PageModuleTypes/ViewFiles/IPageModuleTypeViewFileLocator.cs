using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Allows searching for page module view files in an application
    /// </summary>
    public interface IPageModuleTypeViewFileLocator
    {
        /// <summary>
        /// Returns the full path of a page module view file using the file name, or
        /// null if it could not be found.
        /// </summary>
        /// <param name="pageModuleFileName">The file name (without extension) of the module. E.g 'RawHtml', 'SingleLineText'</param>
        string GetPathByFileName(string pageModuleFileName);

        /// <summary>
        /// Gets the virtual path for a page module template using the
        /// specified file name
        /// </summary>
        /// <param name="pageModuleFileName">The file name of the page module this template is for e.g. 'SingleLineText'</param>
        /// <param name="templateFileName">The file name of the template e.g. 'H1'</param>
        string GetTemplatePathByTemplateFileName(string pageModuleFileName, string templateFileName);

        /// <summary>
        /// Gets a collection of paths to template files for the specified page module type.
        /// </summary>
        /// <param name="pageModuleFileName">The file name of the page module e.g. 'SingleLineText'</param>
        IEnumerable<string> GetAllTemplatePathByModuleFileName(string pageModuleFileName);
    }
}
