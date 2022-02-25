using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    public class CustomEntityRoutingRulesApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;

        public CustomEntityRoutingRulesApiController(
            IApiResponseHelper apiResponseHelper
            )
        {
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get()
        {
            return await _apiResponseHelper.RunQueryAsync(new GetAllCustomEntityRoutingRulesQuery());
        }
    }
}