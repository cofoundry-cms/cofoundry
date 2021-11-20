using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Web;
using Cofoundry.Samples.SPASite.Domain;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;

namespace Cofoundry.Samples.SPASite
{
    [Route("api/features")]
    public class FeaturesApiController : ControllerBase
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IApiResponseHelper _apiResponseHelper;

        public FeaturesApiController(
            IDomainRepository domainRepository,
            IApiResponseHelper apiResponseHelper
            )
        {
            _domainRepository = domainRepository;
            _domainRepository = domainRepository;
            _apiResponseHelper = apiResponseHelper;
        }

        [HttpGet("")]
        public async Task<JsonResult> Get()
        {
            var query = new GetAllFeaturesQuery();
            var results = await _domainRepository.ExecuteQueryAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(results);
        }
    }
}