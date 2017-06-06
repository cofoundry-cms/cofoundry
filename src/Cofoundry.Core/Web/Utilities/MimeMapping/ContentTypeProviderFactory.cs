using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Factory for creating the IContentTypeProvider that gets registered 
    /// with the DI container as a single instance.
    /// </summary>
    public class ContentTypeProviderFactory : IContentTypeProviderFactory
    { 
        private readonly IEnumerable<IMimeTypeRegistration> _mimeTypeRegistrations;

        public ContentTypeProviderFactory(
            IEnumerable<IMimeTypeRegistration> mimeTypeRegistrations
            )
        {
            _mimeTypeRegistrations = mimeTypeRegistrations;
        }

        public IContentTypeProvider Create()
        {
            var provider = new FileExtensionContentTypeProvider();
            var context = new MimeTypeRegistrationContext(provider);

            foreach (var mimeTypeRegistration in _mimeTypeRegistrations)
            {
                mimeTypeRegistration.Register(context);
            }

            return provider;
        }
    }
}
