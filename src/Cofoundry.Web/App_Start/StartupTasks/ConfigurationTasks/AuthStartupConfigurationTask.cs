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
        }
    }
}