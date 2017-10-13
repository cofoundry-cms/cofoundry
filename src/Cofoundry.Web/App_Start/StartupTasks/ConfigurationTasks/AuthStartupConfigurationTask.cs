using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Cofoundry.Web
{
    public class AuthStartupConfigurationTask : IStartupConfigurationTask
    {
        #region constructor

        private readonly IUserAreaRepository _userAreaRepository;

        public AuthStartupConfigurationTask(
            IUserAreaRepository userAreaRepository
            )
        {
            _userAreaRepository = userAreaRepository;
        }

        #endregion

        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Early; }
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            //foreach (var userAreaDefinition in _userAreaRepository.GetAll())
            //{
            //    var cookieOptions = new CookieAuthenticationOptions();
            //    cookieOptions.AuthenticationScheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaDefinition.UserAreaCode);

            //    // Share the cookie name between user areas, because you should only be able to log into one at a time,
            //    cookieOptions.CookieName = "CF_AUTH";

            //    // NB: When adding multiple authentication middleware you should ensure that no middleware is configured to run automatically
            //    // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme
            //    // The problem with this is that we can't auth without applying an auth filter, which 
            //    // is imptractical
            //    cookieOptions.AutomaticAuthenticate = true;
            //    //cookieOptions.AutomaticChallenge = false;

            //    if (!string.IsNullOrWhiteSpace(userAreaDefinition.LoginPath))
            //    {
            //        cookieOptions.LoginPath = userAreaDefinition.LoginPath;
            //    }

            //    if (!string.IsNullOrWhiteSpace(userAreaDefinition.LogoutPath))
            //    {
            //        cookieOptions.LogoutPath = userAreaDefinition.LogoutPath;
            //    }

            //    if (!string.IsNullOrWhiteSpace(userAreaDefinition.AccessDeniedPath))
            //    {
            //        cookieOptions.AccessDeniedPath = userAreaDefinition.AccessDeniedPath;
            //    }

            //    app.UseCookieAuthentication(cookieOptions);
            //}
        }
    }
}