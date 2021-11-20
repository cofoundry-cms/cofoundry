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
    [Route("api/cats")]
    [AutoValidateAntiforgeryToken]
    public class CatsApiController : ControllerBase
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IApiResponseHelper _apiResponseHelper;

        public CatsApiController(
            IDomainRepository domainRepository,
            IApiResponseHelper apiResponseHelper
            )
        {
            _domainRepository = domainRepository;
            _apiResponseHelper = apiResponseHelper;
        }

        [HttpGet("")]
        public async Task<JsonResult> Get([FromQuery] SearchCatSummariesQuery query)
        {
            if (query == null) query = new SearchCatSummariesQuery();
            var results = await _domainRepository.ExecuteQueryAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        [HttpGet("{catId:int}")]
        public async Task<JsonResult> Get(int catId)
        {
            var query = new GetCatDetailsByIdQuery(catId);
            var results = await _domainRepository.ExecuteQueryAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        /// <summary>
        /// Note that here we use the standard Authorize attribute to restrict
        /// access to this endpoint because you need to be logged in to 'like' a 
        /// cat
        /// </summary>
        [AuthorizeUserArea(MemberUserArea.Code)]
        [HttpPost("{catId:int}/likes")]
        public Task<JsonResult> Like(int catId)
        {
            var command = new SetCatLikedCommand()
            {
                CatId = catId,
                IsLiked = true
            };

            // IApiResponseHelper will validate the command and permissions before executing it
            // and return any validation errors in a formatted data object
            return _apiResponseHelper.RunCommandAsync(command);
        }

        [AuthorizeUserArea(MemberUserArea.Code)]
        [HttpDelete("{catId:int}/likes")]
        public Task<JsonResult> UnLike(int catId)
        {
            var command = new SetCatLikedCommand()
            {
                CatId = catId
            };
            return _apiResponseHelper.RunCommandAsync(command);
        }
    }
}