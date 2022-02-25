using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class LocalesApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;

        public LocalesApiController(
            IApiResponseHelper apiResponseHelper
            )
        {
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get()
        {
            var query = new GetAllActiveLocalesQuery();
            return await _apiResponseHelper.RunQueryAsync(query);
        }
    }
}