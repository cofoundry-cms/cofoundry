using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Web;
using Cofoundry.Samples.SPASite.Domain;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;

namespace Cofoundry.Samples.SPASite
{
    [Route("api/breeds")]
    public class BreedsApiController : ControllerBase
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IApiResponseHelper _apiResponseHelper;

        public BreedsApiController(
            IDomainRepository domainRepository,
            IApiResponseHelper apiResponseHelper
            )
        {
            _domainRepository = domainRepository;
            _apiResponseHelper = apiResponseHelper;
        }

        [HttpGet("")]
        public async Task<JsonResult> Get()
        {
            var query = new GetAllBreedsQuery();
            var results = await _domainRepository.ExecuteQueryAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        [HttpGet("{breedId:int}")]
        public async Task<JsonResult> Get(int breedId)
        {
            var query = new GetBreedByIdQuery(breedId);
            var results = await _domainRepository.ExecuteQueryAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(results);
        }
    }
}