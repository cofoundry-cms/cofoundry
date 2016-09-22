using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Configuration;
using Cofoundry.Web.WebApi;
using Cofoundry.Domain;
using Cofoundry.Core.Web;
using Cofoundry.Core;

namespace Cofoundry.Web
{
    public class WebCoreDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterFactory<OptimizationSettings, ConfigurationSettingsFactory<OptimizationSettings>>()
                .RegisterFactory<ContentSettings, ConfigurationSettingsFactory<ContentSettings>>()

                .RegisterPerRequestScope<IControllerResponseHelper, ControllerResponseHelper>()

                .RegisterPerRequestScope<ISettingsViewHelper, SettingsViewHelper>()
                .RegisterPerRequestScope<IRouteViewHelper, RouteViewHelper>()
                .RegisterPerRequestScope<ICurrentUserViewHelper, CurrentUserViewHelper>()
                .RegisterPerRequestScope<IPageTemplateViewFileLocator, PageTemplateViewFileLocator>()
                .RegisterPerRequestScope<IHtmlSanitizerHelper, HtmlSanitizerHelper>()
                .RegisterPerRequestScope<IPageViewModelMapper, PageViewModelMapper>()
                .RegisterPerRequestScope<IJavascriptViewHelper, JavascriptViewHelper>()
                .RegisterPerRequestScope<ICofoundryHtmlHelper, CofoundryHtmlHelper>()
                .RegisterPerRequestScope<IImageAssetUrlResolutionService, ImageAssetUrlResolutionService>()
                .RegisterPerRequestScope<IUserSessionService, UserSessionService>()
                .RegisterPerRequestScope<ILoginService, LoginService>()
                
                .RegisterPerRequestScope<ApiResponseHelper, ApiResponseHelper>()

                .RegisterAll<IPageModuleViewLocationRegistration>()
                .RegisterAll<IModelMetaDataDecorator>()

                .RegisterInstance<ModelMetadataProvider, CofoundryModelMetadataProvider>()
                .RegisterInstance<IAntiCSRFService, AntiCSRFService>()

                .RegisterType<ICustomEntityTemplateSectionTagBuilderFactory, CustomEntityTemplateSectionTagBuilderFactory>()
                .RegisterType<IPageTemplateSectionTagBuilderFactory, PageTemplateSectionTagBuilderFactory>()
                .RegisterType<IModuleRenderer, ModuleRenderer>()
                .RegisterType<IRedirectResponseHelper, RedirectResponseHelper>()
                .RegisterType<IPathResolver, SitePathResolver>(RegistrationOptions.Override(RegistrationOverridePriority.Low))
                .RegisterType<IWebApiStartupConfiguration, WebApiStartupConfiguration>()
                ; 
        }
    }
}
