using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain;
using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain.Internal;
using Cofoundry.Web.Internal;
using Microsoft.AspNetCore.Authorization;
using Cofoundry.Web.Auth.Internal;

namespace Cofoundry.Web.Registration
{
    public class WebCoreDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var singletonRegistrationOptions = RegistrationOptions.SingletonScope();
            var lowPriorityOverrideRegistrationOptions = RegistrationOptions.Override(RegistrationOverridePriority.Low);
            var lowPriorityScopedOverrideRegistrationOptions = RegistrationOptions.Override(RegistrationOverridePriority.Low);
            lowPriorityScopedOverrideRegistrationOptions.Lifetime = InstanceLifetime.Scoped;

            container
                .Register<IControllerResponseHelper, ControllerResponseHelper>()

                .Register<ISettingsViewHelper, SettingsViewHelper>()
                .Register<ICurrentUserViewHelper, CurrentUserViewHelper>(RegistrationOptions.Scoped())
                .Register<IPageTemplateViewFileLocator, PageTemplateViewFileLocator>()
                .Register<IHtmlSanitizerHelper, HtmlSanitizerHelper>()
                .Register<IJavascriptViewHelper, JavascriptViewHelper>()
                .Register<ICofoundryHtmlHelper, CofoundryHtmlHelper>()
                .Register<IUserSessionService, WebUserSessionService>(lowPriorityScopedOverrideRegistrationOptions)
                .Register<IAuthCookieNamespaceProvider, AuthCookieNamespaceProvider>()
                .Register<IAuthorizationHandler, UserAreaAuthorizationHandler>()
                .Register<IAuthorizationHandler, RoleAuthorizationHandler>()
                .Register<IAuthorizationHandler, PermissionAuthorizationHandler>()
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
                .Register<IClientConnectionService, WebClientConnectionService>(lowPriorityOverrideRegistrationOptions)

                .Register<JsonDeltaModelBinder>()
                .Register<IFormFileUploadedFileFactory, FormFileUploadedFileFactory>()

                .RegisterAll<IRouteRegistration>()
                .Register<IRouteInitializer, RouteInitializer>()
                .Register<IResourceLocator, WebsiteResourceLocator>(lowPriorityOverrideRegistrationOptions)
                .Register<IEmptyActionContextFactory, EmptyActionContextFactory>()
                .RegisterFactory<IStaticResourceFileProvider, StaticResourceFileProvider, StaticResourceFileProviderFactory>(singletonRegistrationOptions)

                .Register<IClaimsPrincipalFactory, ClaimsPrincipalFactory>()
                .Register<IClaimsPrincipalValidator, ClaimsPrincipalValidator>()
                .Register<IClaimsPrincipalBuilderContextRepository, ClaimsPrincipalBuilderContextRepository>()
                ; 
        }
    }
}
