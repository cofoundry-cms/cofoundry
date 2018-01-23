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
        [Route("test/test")]
        public IActionResult Test()
        {
            return View(new TestViewModel());
        }
    }
}
