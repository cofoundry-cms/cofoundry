
using Cofoundry.Core.ResourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;

namespace Cofoundry.Web
{
    public class WebsiteResourceFileProviderRegisteration : IResourceFileProviderRegisteration
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public WebsiteResourceFileProviderRegisteration(
            IHostingEnvironment hostingEnvironment
            )
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IEnumerable<IFileProvider> GetFileProviders()
        {
            yield return _hostingEnvironment.ContentRootFileProvider;
        }
    }
}