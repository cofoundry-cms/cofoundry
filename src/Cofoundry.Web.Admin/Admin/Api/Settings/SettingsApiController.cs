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
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public async Task<IActionResult> GetSeoSettings()
        {
            var results = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<SeoSettings>());
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        #endregion

        #region commands

        public Task<JsonResult> PatchGeneralSiteSettings([FromBody] IDelta<UpdateGeneralSiteSettingsCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(delta);
        }

        public Task<JsonResult> PatchSeoSettings([FromBody] IDelta<UpdateSeoSettingsCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(delta);
        }
        
        #endregion
    }
}