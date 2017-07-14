using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Extends the RazorViewEngineOptions configuration adding Cofoundry specific
    /// settings such as extending the FileProviders collection using IResourceFileProviderFactory.
    /// </summary>
    public class CofoundryRazorViewEngineOptionsConfiguration : IRazorViewEngineOptionsConfiguration
    {
        private readonly IResourceFileProviderFactory _resourceFileProviderFactory;

        public CofoundryRazorViewEngineOptionsConfiguration(
            IResourceFileProviderFactory resourceFileProviderFactory
            )
        {
            _resourceFileProviderFactory = resourceFileProviderFactory;
        }

        public void Configure(RazorViewEngineOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.FileProviders.Add(_resourceFileProviderFactory.Create());
        }
    }
}