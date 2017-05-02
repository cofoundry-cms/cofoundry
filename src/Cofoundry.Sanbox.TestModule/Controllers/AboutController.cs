using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Sanbox.TestModule
{
    public class AboutController : Controller
    {
        [Route("about")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
