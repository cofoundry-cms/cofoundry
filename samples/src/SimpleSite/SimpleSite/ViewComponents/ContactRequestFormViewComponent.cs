using Cofoundry.Core.Mail;
using Cofoundry.Core.Validation;
using Microsoft.AspNetCore.Mvc;

namespace SimpleSite;

public class ContactRequestFormViewComponent : ViewComponent
{
    private readonly IMailService _mailService;
    private readonly IModelValidationService _modelValidationService;
    private readonly SimpleSiteSettings _simpleTestSiteSettings;

    public ContactRequestFormViewComponent(
        IMailService mailService,
        IModelValidationService modelValidationService,
        SimpleSiteSettings simpleTestSiteSettings
        )
    {
        _mailService = mailService;
        _modelValidationService = modelValidationService;
        _simpleTestSiteSettings = simpleTestSiteSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var contactRequest = new ContactRequest();

        // ModelBinder is not supported in view components so we have to bind
        // this manually. We have an issue open to try and improve the experience here
        // https://github.com/cofoundry-cms/cofoundry/issues/125

        if (Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            var form = await Request.ReadFormAsync();
            contactRequest.Name = ReadFormField(form, nameof(contactRequest.Name));
            contactRequest.EmailAddress = ReadFormField(form, nameof(contactRequest.EmailAddress));
            contactRequest.Message = ReadFormField(form, nameof(contactRequest.Message));

            ValidateModel(contactRequest);

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
    }

    private static string ReadFormField(IFormCollection form, string fieldName)
    {
        var value = form[fieldName];
        return StringHelper.NullAsEmptyAndTrim(value);
    }

    /// <summary>
    /// Because model binding isn't supported in view components, we have to 
    /// manually validate the model.
    /// </summary>
    private void ValidateModel<TModel>(TModel model)
    {
        var errors = _modelValidationService.GetErrors(model);

        foreach (var error in errors)
        {
            var property = error.Properties.FirstOrDefault() ?? string.Empty;
            ModelState.AddModelError(property, error.Message);
        }
    }
}
