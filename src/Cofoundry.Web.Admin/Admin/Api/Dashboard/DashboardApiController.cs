using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            return await _apiResponseHelper.RunWithResultAsync(async () =>
            {
                return await _dashboardContentService.GetAsync();
            });
        }
    }
}