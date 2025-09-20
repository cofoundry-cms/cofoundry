using Microsoft.AspNetCore.Mvc;

namespace ErrorLoggingExample.Controllers;

public class TestController : Controller
{
    [Route("test")]
    public IActionResult Index()
    {
        throw new Exception("Test Exception.");
    }
}
