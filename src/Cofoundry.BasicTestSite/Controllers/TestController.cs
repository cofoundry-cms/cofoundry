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
        private readonly IPageRepository _pageRepository;

        public TestController(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        [Route("test/test")]
        public async Task<IActionResult> Test()
        {
            var query = new SearchPageRenderSummariesQuery();

            var results = await _pageRepository.SearchPageRenderSummariesAsync(query);

            var query2 = new GetPageRenderSummaryByIdQuery(7);
            var results2 = await _pageRepository.GetPageRenderDetailsByIdAsync(query2);

            var query3 = new GetPageRenderSummariesByIdRangeQuery(new int[] { 2, 5, 7 });
            var results3 = await _pageRepository.GetPageRenderSummariesByIdRangeAsync(query3);

            return View(new TestViewModel());
        }
    }
}
