using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class PermissionsApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;

        public PermissionsApiController(
            IApiResponseHelper apiResponseHelper
            )
        {
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get()
        {
            var query = new GetAllPermissionsQuery();
            return await _apiResponseHelper.RunQueryAsync(query);
        }
    }
}