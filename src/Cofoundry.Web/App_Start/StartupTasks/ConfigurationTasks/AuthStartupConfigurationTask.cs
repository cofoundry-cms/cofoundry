using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Cofoundry.Web
{
    /// <summary>
    /// Adds the asp.net auth middleware into the pipeline.
    /// </summary>
    public class AuthStartupConfigurationTask : IStartupConfigurationTask
    {
        #region constructor

        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public AuthStartupConfigurationTask(
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
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