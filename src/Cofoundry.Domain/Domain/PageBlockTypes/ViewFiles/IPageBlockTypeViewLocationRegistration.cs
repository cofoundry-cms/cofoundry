using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Register a location that contains views for a page block type so that they can 
    /// be put in a non-standard view location.
    /// </summary>
    public interface IPageBlockTypeViewLocationRegistration
    {
        /// <summary>
        /// Returns prefixes of paths to be registered as page block type locations. Only include the start e.g. 'PageBlockTypes' 
        /// rather than 'PageBlockTypes/{0}.cshtml' because the format will be automatically added. You can include any 
        /// number of directory nestings. The block type views themselves should be in the folder format 
        /// '{Path}/{PageBlockTypeName}/{PageBlockTypeName}.cshtml' or '{Path}/{PageBlockTypeName}.cshtml'. Templates should 
        /// be located in the folder format '{Path}/{PageBlockTypeName}/Templates/{PageBlockTypeName}.cshtml' or
        /// '{Path}/Templates/{PageBlockTypeName}.cshtml'
        /// </summary>
        IEnumerable<string> GetPathPrefixes();
    }
}
