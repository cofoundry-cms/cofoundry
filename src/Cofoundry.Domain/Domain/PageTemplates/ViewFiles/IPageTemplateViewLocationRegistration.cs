using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Register a location that contains PageTemplate views, so that they can be put in a non-standard
    /// view location.
    /// </summary>
    public interface IPageTemplateViewLocationRegistration
    {
        /// <summary>
        /// Returns prefixes of paths to be registered as template locations. Only include the start e.g. 'PageTemplates' 
        /// or 'Views/PageTemplates' rather than 'PageTemplates/{0}.cshtml' or 'Views/PageTemplates/{0}.cshtml' because the format 
        /// will be automatically added. You can include any 
        /// number of directory nestings. 
        /// </summary>
        IEnumerable<string> GetPathPrefixes();
    }
}
