using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Web;
using Cofoundry.Samples.SPASite.Domain;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Samples.SPASite
{
    [AuthorizeUserArea(MemberUserArea.Code)]
    [Route("api/members/current")]
    public class CurrentMemberApiController : ControllerBase
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IUserContextService _userContextService;

        public CurrentMemberApiController(
            IDomainRepository domainRepository,
            IApiResponseHelper apiResponseHelper,
            IUserContextService userContextService
            )
        {
            _domainRepository = domainRepository;
            _apiResponseHelper = apiResponseHelper;
            _userContextService = userContextService;
        }

        [HttpGet("cats/liked")]
        public async Task<JsonResult> GetLikedCats()
        {
            // Here we get the userId of the currently logged in member. We could have
            // done this in the query handler, but instead we've chosen to keep the query 
            // flexible so it can be re-used in a more generic fashion
            var userContext = await _userContextService.GetCurrentContextAsync();
            var query = new GetCatSummariesByMemberLikedQuery(userContext.UserId.Value);
            var results = await _domainRepository.ExecuteQueryAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(results);
        }
    }
}