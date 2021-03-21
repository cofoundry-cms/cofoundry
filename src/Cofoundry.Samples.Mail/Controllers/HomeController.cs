using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.Mail.Controllers
{
    /// <summary>
    /// The example homepage contains a simple contact form
    /// to demonstrate sending an email.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IMailService _mailService;

        public HomeController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ContactRequest contactRequest)
        {
            if (ModelState.IsValid)
            {
                // Build the template class
                var template = new ContactRequestMailTemplate();
                template.Request = contactRequest;

                // Send it using the IMailservice abstraction
                // The default implementation will save the email in the 
                // App_Data folder, see readme.md for more info on mail 
                // plugins
                await _mailService.SendAsync("test@example.com", template);

                ViewBag.Message = $"Email sent to {contactRequest.Name} at {contactRequest.EmailAddress}";

                ModelState.Clear();
            }

            return View();
        }
    }
}
