using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain;
using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;

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

                .RegisterType<ICustomEntityTemplateRegionTagBuilderFactory, CustomEntityTemplateRegionTagBuilderFactory>()
                .RegisterType<IPageTemplateRegionTagBuilderFactory, PageTemplateRegionTagBuilderFactory>()
                .RegisterType<IPageBlockRenderer, PageBlockRenderer>()
                .RegisterType<IPathResolver, SitePathResolver>(RegistrationOptions.Override(RegistrationOverridePriority.Low))

                .RegisterType<JsonDeltaModelBinder>()
                .RegisterType<IFormFileUploadedFileFactory, FormFileUploadedFileFactory>()

                .RegisterAll<IRouteRegistration>()
                .RegisterType<IRouteInitializer, RouteInitializer>()
                .RegisterType<IResourceLocator, WebsiteResourceLocator>(RegistrationOptions.Override(RegistrationOverridePriority.Low))
                .RegisterType<IEmptyActionContextFactory, EmptyActionContextFactory>()
                .RegisterInstance<IStaticResourceFileProvider, StaticResourceFileProvider>()
                ; 
        }
    }
}
