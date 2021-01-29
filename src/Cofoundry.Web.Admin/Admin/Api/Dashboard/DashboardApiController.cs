using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class DashboardApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IDashboardContentProvider _dashboardContentService;

        public DashboardApiController(
            IApiResponseHelper apiResponseHelper,
            IDashboardContentProvider dashboardContentService
            )
        {
            _apiResponseHelper = apiResponseHelper;
            _dashboardContentService = dashboardContentService;
        }

        public async Task<JsonResult> Get()
        {
            var result = await _dashboardContentService.GetAsync();
            return _apiResponseHelper.SimpleQueryResponse(result);
        }
    }
}