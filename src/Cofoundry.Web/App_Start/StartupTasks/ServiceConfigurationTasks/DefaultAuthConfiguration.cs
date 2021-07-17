using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Web
{
    /// <summary>
    /// The default auth configuration adds cookie auth for each of the user areas.
    /// </summary>
    public class DefaultAuthConfiguration : IAuthConfiguration
    {
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IAuthCookieNamespaceProvider _authCookieNamespaceProvider;

        public DefaultAuthConfiguration(
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IAuthCookieNamespaceProvider authCookieNamespaceProvider
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _authCookieNamespaceProvider = authCookieNamespaceProvider;
        }

        public void Configure(IMvcBuilder mvcBuilder)
        {
            var services = mvcBuilder.Services;
            var allUserAreas = _userAreaDefinitionRepository.GetAll();

            var defaultSchemaCode = _userAreaDefinitionRepository.GetDefault()?.UserAreaCode;
            if (defaultSchemaCode == null)
            {
                throw new InvalidOperationException("Default user area expected, but none are defined. The Cofoundry Admin user area should be defined at least.");
            }

            var defaultScheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(defaultSchemaCode);

            var authBuilder = mvcBuilder.Services.AddAuthentication(defaultScheme);
            var cookieNamespace = _authCookieNamespaceProvider.GetNamespace();

            foreach (var userAreaDefinition in allUserAreas)
            {
                var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaDefinition.UserAreaCode);

                authBuilder
                    .AddCookie(scheme, cookieOptions =>
                    {
                        cookieOptions.Cookie.Name = cookieNamespace + userAreaDefinition.UserAreaCode;
                        cookieOptions.Cookie.HttpOnly = true;
                        cookieOptions.Cookie.IsEssential = true;
                        cookieOptions.Cookie.SameSite = SameSiteMode.Lax;

                        if (!string.IsNullOrWhiteSpace(userAreaDefinition.LoginPath))
                        {
                            cookieOptions.LoginPath = userAreaDefinition.LoginPath;
                        }
                    });
            }

            mvcBuilder.Services.AddAuthorization();
        }
    }
}
