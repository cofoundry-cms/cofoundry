using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Allows searching for page block type view files in an application
    /// </summary>
    public interface IPageBlockTypeViewFileLocator
    {
        /// <summary>
        /// Returns the full path of a page block type view file using the file name, or
        /// null if it could not be found.
        /// </summary>
        /// <param name="pageBlockTypeFileName">The file name (without extension) of the page block type. E.g 'RawHtml', 'SingleLineText'</param>
        string GetPathByFileName(string pageBlockTypeFileName);

        /// <summary>
        /// Gets the virtual path for a page block type template using the
        /// specified file name
        /// </summary>
        /// <param name="pageBlockTypeFileName">The file name of the page block type this template is for e.g. 'SingleLineText'</param>
        /// <param name="templateFileName">The file name of the template e.g. 'H1'</param>
        string GetTemplatePathByTemplateFileName(string pageBlockTypeFileName, string templateFileName);

        /// <summary>
        /// Gets a collection of paths to template files for the specified page block type.
        /// </summary>
        /// <param name="pageBlockTypeFileName">The file name of the page block e.g. 'SingleLineText'</param>
        IEnumerable<string> GetAllTemplatePathsByFileName(string pageBlockTypeFileName);
    }
}
