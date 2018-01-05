using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.StaticFiles;

namespace Cofoundry.Web
{
    /// <summary>
    /// Adds the asp.net static files middleware with a default configuration. You can 
    /// customize this by overriding the default IStaticFileOptionsConfiguration 
    /// implementation using DI.
    /// </summary>
    public class StaticFileStartupConfigurationTask : IStartupConfigurationTask
    {
        #region constructor

        private readonly IStaticResourceFileProvider _staticResourceFileProvider;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IStaticFileOptionsConfiguration _staticFileOptionsConfiguration;

        public StaticFileStartupConfigurationTask(
            IStaticResourceFileProvider staticResourceFileProvider,
            IContentTypeProvider contentTypeProvider,
            IStaticFileOptionsConfiguration staticFileOptionsConfiguration
            )
        {
            _staticResourceFileProvider = staticResourceFileProvider;
            _contentTypeProvider = contentTypeProvider;
            _staticFileOptionsConfiguration = staticFileOptionsConfiguration;
        }

        #endregion

        public int Ordering
        {
            get { return (int) StartupTaskOrdering.Early; }
        }

        public void Configure(IApplicationBuilder app)
        {
            RegisterStaticFiles(app);
        }

        private void RegisterStaticFiles(IApplicationBuilder app)
        {
            var options = new StaticFileOptions()
            {
                FileProvider = _staticResourceFileProvider,
                ContentTypeProvider = _contentTypeProvider,
            };

            _staticFileOptionsConfiguration.Configure(options);

            app.UseStaticFiles(options);
        }
    }
}