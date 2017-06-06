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
                //.RegisterType<RequestBasedSiteUrlResolver>()
                .RegisterType<ConfigBasedSiteUrlResolver>()
                .RegisterType<ISiteUrlResolver, CompositeSiteUrlResolver>()
                .RegisterType<IHtmlSanitizer, HtmlSanitizer>()
                .RegisterType<IDefaultHtmlSanitizationRuleSetFactory, DefaultHtmlSanitizationRuleSetFactory>()
                .RegisterType<IMimeTypeService, MimeTypeService>()
                .RegisterType<IContentTypeProviderFactory, ContentTypeProviderFactory>()
                .RegisterFactory<IContentTypeProvider, IContentTypeProviderFactory>(new RegistrationOptions() { InstanceScope = InstanceScope.Singleton })
                .RegisterAll<IMimeTypeRegistration>()
                ;
        }
    }
}
