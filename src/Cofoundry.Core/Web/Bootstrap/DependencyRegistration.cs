using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Configuration;
using Microsoft.AspNetCore.StaticFiles;

namespace Cofoundry.Core.Web.Bootstrap
{
    public class DependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<RequestBasedSiteUrlResolver>()
                .Register<ConfigBasedSiteUrlResolver>()
                .Register<ISiteUrlResolver, CompositeSiteUrlResolver>()
                .Register<IHtmlSanitizer, HtmlSanitizer>()
                .Register<IDefaultHtmlSanitizationRuleSetFactory, DefaultHtmlSanitizationRuleSetFactory>()
                .Register<IMimeTypeService, MimeTypeService>()
                .RegisterSingleton<IContentTypeProviderFactory, ContentTypeProviderFactory>()
                .RegisterFactory<IContentTypeProvider, IContentTypeProviderFactory>(RegistrationOptions.SingletonScope())
                .RegisterAll<IMimeTypeRegistration>()
                ;
        }
    }
}
