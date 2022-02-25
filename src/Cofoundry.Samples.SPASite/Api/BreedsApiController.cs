using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Web;
using Cofoundry.Samples.SPASite.Domain;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Samples.SPASite
{
    [Route("api/breeds")]
    public class BreedsApiController : ControllerBase
    {
        private readonly IApiResponseHelper _apiResponseHelper;

        public BreedsApiController(
            IApiResponseHelper apiResponseHelper
            )
        {
            _apiResponseHelper = apiResponseHelper;
        }

        [HttpGet("")]
        public async Task<JsonResult> Get()
        {
            return await _apiResponseHelper.RunQueryAsync(new GetAllBreedsQuery());
        }

        [HttpGet("{breedId:int}")]
        public async Task<JsonResult> Get(int breedId)
        {
            var query = new GetBreedByIdQuery(breedId);
            return await _apiResponseHelper.RunQueryAsync(query);
        }
    }
}