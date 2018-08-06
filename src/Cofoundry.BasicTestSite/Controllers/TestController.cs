using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    public class TestViewModel
    {
        public int TestID { get; set; }
    }

    public class TestController : Controller
    {
        private readonly IQueryExecutor _queryExecutor;

        public TestController(IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        [Route("test/test")]
        public async Task<IActionResult> Test()
        {
            var query = new SearchPageRenderSummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);

            query.SortBy = PageQuerySortType.CreateDate;
            query.PageDirectoryId = 1;
            var results2 = await _queryExecutor.ExecuteAsync(query);

            return View(new TestViewModel());
        }
    }
}
