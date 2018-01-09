using Cofoundry.Domain.CQS;
using Cofoundry.Web;
using Cofoundry.Web.Admin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    [AdminApiRoute("test")]
    public class TestApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{clientId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public TestApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region routes

        [HttpGet]
        public IActionResult Get()
        {
            var results = new List<object>()
            {
                new { Id = 1, Title = "Test 1" },
                new { Id = 2, Title = "Test 2" },
                new { Id = 3, Title = "Test 3" },
                new { Id = 4, Title = "Test 4" },
                new { Id = 5, Title = "Test 5" },
                new { Id = 6, Title = "Test 6" },
                new { Id = 7, Title = "Test 7" },
                new { Id = 8, Title = "Test 8" },
                new { Id = 9, Title = "Test 9" },
                new { Id = 10, Title = "Test 10" }
            };
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion
    }
}
