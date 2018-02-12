using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.ResourceFiles
{
    /// <summary>
    /// Used to register folders which contain embedded resources that can be 
    /// exposed publically in a website e.g. images or css files.
    /// </summary>
    /// <remarks>
    /// This is in core principally to account for assemblies that may contain email 
    /// templates that may need to be included in a web app and a background process,
    /// whereby we don't want to include the full web stack in the background process.
    /// </remarks>
    public interface IEmbeddedResourceRouteRegistration
    {
        /// <summary>
        /// Returns paths used to locate folders which
        /// contains embedded resources. E.g. '/parent/child/content'
        /// </summary>
        IEnumerable<EmbeddedResourcePath> GetEmbeddedResourcePaths();
    }
}
