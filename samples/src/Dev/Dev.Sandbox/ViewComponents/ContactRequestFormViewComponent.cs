using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Mvc;

namespace Dev.Sandbox;

public class ContactRequestFormViewComponent : ViewComponent
{
    private readonly IMailService _mailService;
    private readonly SandboxSiteSettings _simpleTestSiteSettings;

    public ContactRequestFormViewComponent(
        IMailService mailService,
        SandboxSiteSettings simpleTestSiteSettings)
    {
        _mailService = mailService;
        _simpleTestSiteSettings = simpleTestSiteSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var contactRequest = new ContactRequest();

        if (Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            contactRequest.Name = GetFormValueOrDefault(nameof(contactRequest.Name));
            contactRequest.EmailAddress = GetFormValueOrDefault(nameof(contactRequest.EmailAddress));
            contactRequest.Message = GetFormValueOrDefault(nameof(contactRequest.Message));

            // TODO: Update model binding, Validate model
            if (ModelState.IsValid)
            {
                // Send admin confirmation
                var template = new ContactRequestMailTemplate
                {
                    Request = contactRequest
                };
                await _mailService.SendAsync(_simpleTestSiteSettings.ContactRequestNotificationToAddress, template);

                return View("ContactSuccess");
            }
        }

        return View(contactRequest);

        string GetFormValueOrDefault(string propertyName)
        {
            string? value = Request.Form[propertyName];

            return value ?? string.Empty;
        }
    }
}
