using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Web.Auth.Internal;
using Cofoundry.Web.Internal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Web.Extendable
{
    /// <summary>
    /// <para>
    /// Configures MVC authentication during the Cofoundry startup process. 
    /// This default implementation of <see cref="IAuthConfiguration"/> adds 
    /// cookie auth for each of the user areas.
    /// </para>
    /// <para>
    /// If you want to customize the auth registration process with your own
    /// <see cref="IAuthConfiguration"/> implementation, <see cref="DefaultAuthConfiguration"/>
    /// can be used as a base class with several methods that can be overridden
    /// based on the aspects you want to customize.
    /// </para>
    /// </summary>
    /// <inheritdoc/>
    public class DefaultAuthConfiguration : IAuthConfiguration
    {
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IRoleDefinitionRepository _roleDefinitionRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IAuthCookieNamespaceProvider _authCookieNamespaceProvider;

        public DefaultAuthConfiguration(
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IRoleDefinitionRepository roleDefinitionRepository,
            IPermissionRepository permissionRepository,
            IAuthCookieNamespaceProvider authCookieNamespaceProvider
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _roleDefinitionRepository = roleDefinitionRepository;
            _permissionRepository = permissionRepository;
            _authCookieNamespaceProvider = authCookieNamespaceProvider;
        }

        public virtual void Configure(IMvcBuilder mvcBuilder)
        {
            var defaultScheme = GetDefaultScheme();
            var authBuilder = mvcBuilder.Services.AddAuthentication(defaultScheme);
            ConfigureAuthBuilder(authBuilder);

            mvcBuilder.Services.AddAuthorization(ConfigurePolicies);
        }

        protected virtual void ConfigureAuthBuilder(AuthenticationBuilder authBuilder)
        {
            var allUserAreas = _userAreaDefinitionRepository.GetAll();

            foreach (var userArea in allUserAreas)
            {
                var cookieNamespace = _authCookieNamespaceProvider.GetNamespace(userArea.UserAreaCode);
                var scheme = AuthenticationSchemeNames.UserArea(userArea.UserAreaCode);
                var options = new UserAreaSchemeRegistrationOptions(userArea, scheme, cookieNamespace);

                ConfigureUserAreaScheme(authBuilder, options);
            }
        }

        /// <summary>
        /// Returns the authenitcation scheme name that should be set as the default
        /// in a call to <see cref="AuthenticationServiceCollectionExtensions.AddAuthentication"/>.
        /// Override this if you want to change the default scheme.
        /// </summary>
        protected virtual string GetDefaultScheme()
        {
            var defaultSchemeCode = _userAreaDefinitionRepository.GetDefault()?.UserAreaCode;
            if (defaultSchemeCode == null)
            {
                throw new InvalidOperationException("Default user area expected, but none are defined. The Cofoundry Admin user area should be defined at least.");
            }

            var defaultScheme = AuthenticationSchemeNames.UserArea(defaultSchemeCode);

            return defaultScheme;
        }

        /// <summary>
        /// This method is called for each user area registered in Cofoundry. The default implementation
        /// adds a cookie authentication scheme, using <see cref="ConfigureCookieOptions"/> to configure
        /// the cookie options. Override this if you want to completely customize the configuration of
        /// an authentication scheme e.g. replacing cookie authentication.
        /// </summary>
        /// <param name="authenticationBuilder">The builder to add the user area authentication scheme to.</param>
        /// <param name="schemeRegistrationOptions">Parameters that can be used to configure the authentication scheme.</param>
        protected virtual void ConfigureUserAreaScheme(AuthenticationBuilder authenticationBuilder, UserAreaSchemeRegistrationOptions schemeRegistrationOptions)
        {
            authenticationBuilder
                .AddCookie(schemeRegistrationOptions.Scheme, cookieOptions =>
                {
                    ConfigureCookieOptions(cookieOptions, schemeRegistrationOptions);
                });
        }

        /// <summary>
        /// Configures the cookie options for a cookie-based user area authentication scheme.
        /// Override this to customize the default cookie settings used by Cofoundry.
        /// </summary>
        /// <param name="cookieOptions">Options to be configured.</param>
        /// <param name="schemeRegistrationOptions">Parameters that can be used to configure the authentication scheme.</param>
        protected virtual void ConfigureCookieOptions(CookieAuthenticationOptions cookieOptions, UserAreaSchemeRegistrationOptions schemeRegistrationOptions)
        {
            cookieOptions.Cookie.Name = schemeRegistrationOptions.CookieNamespace + schemeRegistrationOptions.UserArea.UserAreaCode;
            cookieOptions.Cookie.HttpOnly = true;
            cookieOptions.Cookie.IsEssential = true;
            cookieOptions.Cookie.SameSite = SameSiteMode.Lax;
            cookieOptions.Events.OnValidatePrincipal = ValidateCookiePrincipal;

            if (!string.IsNullOrWhiteSpace(schemeRegistrationOptions.UserArea.SignInPath))
            {
                cookieOptions.LoginPath = schemeRegistrationOptions.UserArea.SignInPath;
            }
            else
            {
                cookieOptions.Events.OnRedirectToLogin = DefaultSignInRedirectAction;
            }

            cookieOptions.Events.OnRedirectToAccessDenied = DefaultDenyAction;
        }

        /// <summary>
        /// This method should be applied to the <see cref="CookieAuthenticationOptions.Events.OnValidatePrincipal"/>
        /// event, delegating the validation to the registered <see cref="IClaimsPrincipalValidator"/>.
        /// </summary>
        protected Task ValidateCookiePrincipal(CookieValidatePrincipalContext context)
        {
            var claimsPrincipalValidator = context.HttpContext.RequestServices.GetRequiredService<IClaimsPrincipalValidator>();
            return claimsPrincipalValidator.ValidateAsync(context);
        }

        /// <summary>
        /// <para>
        /// This action runs when <see cref="IUserAreaDefinition.SignInPath"/> is null and the "Challenge" action
        /// is triggered (the user is not authenticated). By default ASP.NET tries to redirect to a default sign in
        /// page that probably won't exist, so in Cofoundry we override this with the <see cref="DefaultDenyAction"/>
        /// which will throw a <see cref="NotPermittedException"/>, deferring to the default error handling 
        /// page to return a 403 (Forbidden) error.
        /// </para>
        /// <para>
        /// Override this to customize this behaviour for the sign in redriect, or override <see cref="DefaultDenyAction"/>
        /// to override the behaviour to both default actions.
        /// </para>
        /// </summary>
        /// <typeparam name="TOptions">
        /// The options type depends on the scheme type invoking the action, but for the default 
        /// implementation this will be <see cref="Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions"/>.
        /// </typeparam>
        /// <param name="redirectContext">The context of the redirect action.</param>
        protected virtual Task DefaultSignInRedirectAction<TOptions>(RedirectContext<TOptions> redirectContext)
            where TOptions : AuthenticationSchemeOptions
        {
            return DefaultDenyAction(redirectContext);
        }

        /// <summary>
        /// <para>
        /// This action runs when a user is not permitted to access a route and the "Forbid" action
        /// is triggered. By default ASP.NET tries and redirect to a specific page, but in Cofoundry
        /// we override this to throw a <see cref="NotPermittedException"/> which is (by default) caught
        /// by the global error handler to return the configured 403 (Forbidden) error page.
        /// </para>
        /// <para>
        /// Override this to customize this behaviour.
        /// </para>
        /// </summary>
        /// <typeparam name="TOptions">
        /// The options type depends on the scheme type invoking the action, but for the default 
        /// implementation this will be <see cref="Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions"/>.
        /// </typeparam>
        /// <param name="redirectContext">The context of the redirect action.</param>
        protected virtual Task DefaultDenyAction<TOptions>(RedirectContext<TOptions> redirectContext)
            where TOptions : AuthenticationSchemeOptions
        {
            // here we prefer to throw an exception that will get caught by the 
            // defaut error handler rather than redirect to a page we have to configure.
            throw new NotPermittedException();
        }

        /// <summary>
        /// Policy configuration delegate passed into <see cref="PolicyServiceCollectionExtensions.AddAuthorization"/>.
        /// The default implementation adds a series of authorization policies for Cofoundry roles and permissions.
        /// </summary>
        /// <param name="options"></param>
        protected virtual void ConfigurePolicies(AuthorizationOptions options)
        {
            foreach (var userArea in _userAreaDefinitionRepository.GetAll())
            {
                var policyName = AuthorizationPolicyNames.UserArea(userArea.UserAreaCode);
                var authRequirement = new UserAreaAuthorizationRequirement(userArea.UserAreaCode);
                options.AddPolicy(policyName, p => p.AddRequirements(authRequirement));
            }

            foreach (var role in _roleDefinitionRepository.GetAll())
            {
                var policyName = AuthorizationPolicyNames.Role(role.UserAreaCode, role.RoleCode);
                var authRequirement = new RoleAuthorizationRequirement(role.UserAreaCode, role.RoleCode);
                options.AddPolicy(policyName, p => p.AddRequirements(authRequirement));
            }

            foreach (var permission in _permissionRepository.GetAll())
            {
                var policyName = AuthorizationPolicyNames.Permission(permission);
                var authRequirement = new PermissionAuthorizationRequirement(permission);
                options.AddPolicy(policyName, p => p.AddRequirements(authRequirement));
            }
        }
    }
}
