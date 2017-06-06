using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Used for registering mime types for use with the IMimeTypeService
    /// and IContentTypeProviderFactory.
    /// </summary>
    public interface IMimeTypeRegistrationContext
    {
        /// <summary>
        /// True if the file extension has already been registered; otherwise false.
        /// </summary>
        bool IsDefined(string fileExtension);

        /// <summary>
        /// Adds or updates an existing a mimetype registration for a specific file 
        /// extension.
        /// </summary>
        /// <param name="fileExtension">
        /// The file extension to register e.g. '.jpg'. A dot should prefix the file 
        /// extension, but it will be added if it has not been included.
        /// </param>
        /// <param name="mimeType">The mime type to assign to the file extension e.g. 'image/jpeg'</param>
        void AddOrUpdate(string fileExtension, string mimeType);
    }
}
