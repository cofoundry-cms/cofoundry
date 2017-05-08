using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Sandbox.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(
            IOptions<MvcViewOptions> viewOptions,
            IOptions<RazorViewEngineOptions> razorEngineOptions,
            IRazorViewEngine razorViewEngine,
            IHostingEnvironment hostingEnvironment,
            IEnumerable<IExampleClass> examples,
            IExampleClass example,
            IServiceProvider serviceProvider
            )
        {
            var options = viewOptions.Value;
            IViewEngine viewEngine = viewOptions.Value.ViewEngines.FirstOrDefault();
            var razorOptions = razorEngineOptions.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
