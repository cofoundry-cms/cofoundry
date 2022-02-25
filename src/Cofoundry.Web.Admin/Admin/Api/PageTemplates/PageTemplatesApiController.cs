using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class PageTemplatesApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;

        public PageTemplatesApiController(
            IApiResponseHelper apiResponseHelper
            )
        {
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get([FromQuery] SearchPageTemplateSummariesQuery query)
        {
            if (query == null) query = new SearchPageTemplateSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public async Task<JsonResult> GetById(int id)
        {
            var query = new GetPageTemplateDetailsByIdQuery(id);
            return await _apiResponseHelper.RunQueryAsync(query);
        }
    }
}