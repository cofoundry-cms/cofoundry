using Microsoft.AspNetCore.Mvc;

namespace RegistrationAndVerificationSample.Controllers;

public class HomeController : Controller
{
    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }
}