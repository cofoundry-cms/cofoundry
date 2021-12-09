using Cofoundry.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class UserAreasApiController : BaseAdminApiController
    {
        private readonly IAdvancedContentRepository _contentRepository;
        private readonly IApiResponseHelper _apiResponseHelper;

        public UserAreasApiController(
            IAdvancedContentRepository contentRepository,
            IApiResponseHelper apiResponseHelper
            )
        {
            _contentRepository = contentRepository;
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get()
        {
            var results = await _contentRepository
                .UserAreas()
                .GetAll()
                .AsMicroSummaries()
                .ExecuteAsync();

            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        /// <remarks>
        /// Note that the password policy needs to be publicly accessible
        /// for the initial setup and password recovery pages.
        /// </remarks>
        [AllowAnonymous]
        public async Task<JsonResult> GetPasswordPolicy(string userAreaCode)
        {
            var results = await _contentRepository
                .UserAreas()
                .PasswordPolicies()
                .GetByCode(userAreaCode)
                .AsDescription()
                .ExecuteAsync();

            return _apiResponseHelper.SimpleQueryResponse(results);
        }
    }
}