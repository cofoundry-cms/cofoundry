using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Implement this interface to add a mime type to the central registration
    /// used in IMimeTypeService and IContentTypeProviderFactory. Implementations will 
    /// be automatically picked up and registered at startup when using the default 
    /// Cofoundry application startup process.
    /// </summary>
    public interface IMimeTypeRegistration
    {
        /// <summary>
        /// Use the context object to add or update additional mime types
        /// with the central registry.
        /// </summary>
        /// <param name="context">The context is used to update the central registry.</param>
        void Register(IMimeTypeRegistrationContext context);
    }
}
