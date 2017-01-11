using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("settings")]
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

        [HttpGet]
        [Route(GENERAL_SITE_SETTINGS_ROUTE)]
        public async Task<IHttpActionResult> GetGeneralSiteSettings()
        {
            var results = await _queryExecutor.GetAsync<GeneralSiteSettings>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet]
        [Route(SEO_SETTINGS_ROUTE)]
        public async Task<IHttpActionResult> GetSeoSettings()
        {
            var results = await _queryExecutor.GetAsync<SeoSettings>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #region commands

        [HttpPatch]
        [Route(GENERAL_SITE_SETTINGS_ROUTE)]
        public async Task<IHttpActionResult> PatchGeneralSiteSettings(Delta<UpdateGeneralSiteSettingsCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, delta);
        }

        [HttpPatch]
        [Route(SEO_SETTINGS_ROUTE)]
        public async Task<IHttpActionResult> PatchSeoSettings(Delta<UpdateSeoSettingsCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, delta);
        }

        [HttpPatch]
        [Route(VISUAL_EDITOR_SETTINGS_ROUTE)]
        public async Task<IHttpActionResult> PatchVisualEditorSettings(Delta<UpdateVisualEditorSettingsCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, delta);
        }
        
        #endregion

        #endregion
    }
}