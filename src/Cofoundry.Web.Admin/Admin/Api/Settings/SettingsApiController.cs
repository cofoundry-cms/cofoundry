using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("settings")]
    public class SettingsApiController : BaseAdminApiController
    {
        #region private member variables

        const string GENERAL_SITE_SETTINGS_ROUTE = "GeneralSite";
        const string SEO_SETTINGS_ROUTE = "Seo";
        const string VISUAL_EDITOR_SETTINGS_ROUTE = "VisualEditor";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public SettingsApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region routes

        #region queries

        [HttpGet(GENERAL_SITE_SETTINGS_ROUTE)]
        public async Task<IActionResult> GetGeneralSiteSettings()
        {
            var results = await _queryExecutor.GetAsync<GeneralSiteSettings>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet(SEO_SETTINGS_ROUTE)]
        public async Task<IActionResult> GetSeoSettings()
        {
            var results = await _queryExecutor.GetAsync<SeoSettings>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #region commands

        [HttpPatch(GENERAL_SITE_SETTINGS_ROUTE)]
        public async Task<IActionResult> PatchGeneralSiteSettings([FromBody] IDelta<UpdateGeneralSiteSettingsCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, delta);
        }

        [HttpPatch(SEO_SETTINGS_ROUTE)]
        public async Task<IActionResult> PatchSeoSettings([FromBody] IDelta<UpdateSeoSettingsCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, delta);
        }

        [HttpPatch(VISUAL_EDITOR_SETTINGS_ROUTE)]
        public async Task<IActionResult> PatchVisualEditorSettings([FromBody] IDelta<UpdateVisualEditorSettingsCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, delta);
        }
        
        #endregion

        #endregion
    }
}