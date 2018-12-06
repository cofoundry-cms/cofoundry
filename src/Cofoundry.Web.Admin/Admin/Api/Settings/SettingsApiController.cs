using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    public class SettingsApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public SettingsApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #region queries

        public async Task<IActionResult> GetGeneralSiteSettings()
        {
            var results = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<GeneralSiteSettings>());
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        public async Task<IActionResult> GetSeoSettings()
        {
            var results = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<SeoSettings>());
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #region commands
        // Adding verbs ere fixes things
        // https://blogs.msdn.microsoft.com/webdev/2018/08/27/asp-net-core-2-2-0-preview1-endpoint-routing/

        [HttpPatch]
        public async Task<IActionResult> PatchGeneralSiteSettings([FromBody] IDelta<UpdateGeneralSiteSettingsCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, delta);
        }

        [HttpPatch]
        public async Task<IActionResult> PatchSeoSettings([FromBody] IDelta<UpdateSeoSettingsCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, delta);
        }
        
        #endregion
    }
}