using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.BasicTestSite
{
    public class TestController : Controller
    {
        private readonly CofoundryDbContext _dbContext;

        public TestController(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        [Route("test/test")]
        public IActionResult Test()
        {
            return View();
        }
    }
}
