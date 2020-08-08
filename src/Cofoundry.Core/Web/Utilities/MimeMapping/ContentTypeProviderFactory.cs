using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web.Internal
{
    /// <summary>
    /// Factory for creating the IContentTypeProvider that gets registered 
    /// with the DI container as a single instance.
    /// </summary>
    public class ContentTypeProviderFactory : IContentTypeProviderFactory
    { 
        private readonly IServiceProvider _serviceProvider;

        public ContentTypeProviderFactory(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        public IContentTypeProvider Create()
        {
            // IContentTypeProvider/Factory is registered singleton, so create a scope here
            using (var scope = _serviceProvider.CreateScope())
            {
                var mimeTypeRegistrations = scope.ServiceProvider.GetRequiredService<IEnumerable<IMimeTypeRegistration>>();

                var provider = new FileExtensionContentTypeProvider();
                var context = new MimeTypeRegistrationContext(provider);

                foreach (var mimeTypeRegistration in mimeTypeRegistrations)
                {
                    mimeTypeRegistration.Register(context);
                }

                return provider;
            }
        }
    }
}
