using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web.Internal
{
    /// <summary>
    /// Used for registering mime types for use with the IMimeTypeService
    /// and IContentTypeProviderFactory.
    /// </summary>
    public class MimeTypeRegistrationContext : IMimeTypeRegistrationContext
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public MimeTypeRegistrationContext(
            FileExtensionContentTypeProvider fileExtensionContentTypeProvider
            )
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
        }

        /// <summary>
        /// True if the file extension has already been registered; otherwise false.
        /// Case insensitive.
        /// </summary>
        public virtual bool IsDefined(string fileExtension)
        {
            var formattedExtension = FormatFileExtension(fileExtension);
            return _fileExtensionContentTypeProvider.Mappings.ContainsKey(formattedExtension);
        }

        /// <summary>
        /// Adds or updates an existing a mime type registration for a specific file 
        /// extension.
        /// </summary>
        /// <param name="fileExtension">
        /// The file extension to register e.g. '.jpg'. A dot should prefix the file 
        /// extension, but it will be added if it has not been included.
        /// </param>
        /// <param name="mimeType">The mime type to assign to the file extension e.g. 'image/jpeg'</param>
        public virtual void AddOrUpdate(string fileExtension, string mimeType)
        {
            if (string.IsNullOrWhiteSpace(mimeType)) throw new ArgumentException("Mime type cannot be empty.", nameof(mimeType));

            var formattedExtension = FormatFileExtension(fileExtension);
            _fileExtensionContentTypeProvider.Mappings[formattedExtension] = mimeType;
        }

        private string FormatFileExtension(string fileExtension)
        {
            var trimmedExtension = fileExtension?.Trim('.');

            if (string.IsNullOrWhiteSpace(fileExtension?.Trim('.')))
            {
                throw new ArgumentException("Cannot register an empty file extension.", nameof(fileExtension));
            }

            return '.' + trimmedExtension;
        }
    }
}
