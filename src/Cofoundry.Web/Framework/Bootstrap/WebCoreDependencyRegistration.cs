using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain;
using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Web.Registration
{
    public class WebCoreDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var singletonRegistrationOptions = RegistrationOptions.SingletonScope();
            var lowPriorityOverrideRegistrationOptions = RegistrationOptions.Override(RegistrationOverridePriority.Low);

            container
                .Register<IControllerResponseHelper, ControllerResponseHelper>()

                .Register<ISettingsViewHelper, SettingsViewHelper>()
                .Register<ICurrentUserViewHelper, CurrentUserViewHelper>(RegistrationOptions.Scoped())
                .Register<IPageTemplateViewFileLocator, PageTemplateViewFileLocator>()
                .Register<IHtmlSanitizerHelper, HtmlSanitizerHelper>()
                .Register<IJavascriptViewHelper, JavascriptViewHelper>()
                .Register<ICofoundryHtmlHelper, CofoundryHtmlHelper>()
                .Register<IUserSessionService, UserSessionService>(RegistrationOptions.Scoped())
                .Register<IAuthCookieNamespaceProvider, AuthCookieNamespaceProvider>()
                .Register<IVisualEditorStateService, DefaultVisualEditorStateService>()
                .Register<IVisualEditorStateCache, VisualEditorStateCache>(RegistrationOptions.Scoped())

                .Register<IPageViewModelMapper, PageViewModelMapper>()
                .Register<IPageViewModelFactory, PageViewModelFactory>()
                .Register<IPageViewModelBuilder, PageViewModelBuilder>()
                
                .Register<IApiResponseHelper, ApiResponseHelper>()

                .Register<ICustomEntityTemplateRegionTagBuilderFactory, CustomEntityTemplateRegionTagBuilderFactory>()
                .Register<IPageTemplateRegionTagBuilderFactory, PageTemplateRegionTagBuilderFactory>()
                .Register<IPageBlockRenderer, PageBlockRenderer>()
                .Register<IPathResolver, SitePathResolver>(lowPriorityOverrideRegistrationOptions)

                .Register<JsonDeltaModelBinder>()
                .Register<IFormFileUploadedFileFactory, FormFileUploadedFileFactory>()

                .RegisterAll<IRouteRegistration>()
                .Register<IRouteInitializer, RouteInitializer>()
                .Register<IResourceLocator, WebsiteResourceLocator>(lowPriorityOverrideRegistrationOptions)
                .Register<IEmptyActionContextFactory, EmptyActionContextFactory>()
                .RegisterFactory<IStaticResourceFileProvider, StaticResourceFileProvider, StaticResourceFileProviderFactory>(singletonRegistrationOptions)
                ; 
        }
    }
}
