using Cofoundry.Web.ModularMvc;
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
    public class StaticFileStartupConfigurationTask : IStartupConfigurationTask
    {
        #region constructor

        private readonly IStaticResourceFileProvider _staticResourceFileProvider;
        private readonly IContentTypeProvider _contentTypeProvider;

        public StaticFileStartupConfigurationTask(
            IStaticResourceFileProvider staticResourceFileProvider,
            IContentTypeProvider contentTypeProvider
            )
        {
            _staticResourceFileProvider = staticResourceFileProvider;
            _contentTypeProvider = contentTypeProvider;
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
            // perhaps use a StaticFileOptions factory?
            // or expose all the providers with settings on the _staticResourceFileProvider
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = _staticResourceFileProvider,
                ContentTypeProvider = _contentTypeProvider
            });
        }
    }
}