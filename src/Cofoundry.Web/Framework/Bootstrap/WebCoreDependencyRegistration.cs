using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.DependencyInjection;
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
                .RegisterType<IControllerResponseHelper, ControllerResponseHelper>()

                .RegisterType<ISettingsViewHelper, SettingsViewHelper>()
                .RegisterType<ICurrentUserViewHelper, CurrentUserViewHelper>()
                .RegisterType<IPageTemplateViewFileLocator, PageTemplateViewFileLocator>()
                .RegisterType<IHtmlSanitizerHelper, HtmlSanitizerHelper>()
                .RegisterType<IJavascriptViewHelper, JavascriptViewHelper>()
                .RegisterType<ICofoundryHtmlHelper, CofoundryHtmlHelper>()
                .RegisterType<IUserSessionService, UserSessionService>()

                .RegisterType<IPageViewModelMapper, PageViewModelMapper>()
                .RegisterType<IPageViewModelFactory, PageViewModelFactory>()
                .RegisterType<IPageViewModelBuilder, PageViewModelBuilder>()
                
                .RegisterType<IApiResponseHelper, ApiResponseHelper>()
                
                .RegisterInstance<IAntiCSRFService, AntiCSRFService>()

                .RegisterType<ICustomEntityTemplateSectionTagBuilderFactory, CustomEntityTemplateSectionTagBuilderFactory>()
                .RegisterType<IPageTemplateSectionTagBuilderFactory, PageTemplateSectionTagBuilderFactory>()
                .RegisterType<IPageModuleRenderer, PageModuleRenderer>()
                .RegisterType<IPathResolver, SitePathResolver>(RegistrationOptions.Override(RegistrationOverridePriority.Low))

                .RegisterType<JsonDeltaModelBinder>()
                ; 
        }
    }
}
