using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SendGridExample.Pages;

public class IndexModel : PageModel
{
    private readonly IMailService _mailService;

    public IndexModel(IMailService mailService)
    {
        _mailService = mailService;
    }

    [BindProperty]
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string EmailAddress { get; set; } = string.Empty;

    [BindProperty]
    public string DisplayName { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Message { get; set; } = string.Empty;

    public string? Result { get; set; }

    public void OnGet()
    {
    }

    public async Task OnPost()
    {
        if (!ModelState.IsValid)
        {
            return;
        }

        var template = new ExampleMailTemplate()
        {
            Message = Message
        };

        await _mailService.SendAsync(EmailAddress, template);

        Result = $"Message sent to {EmailAddress}";
    }
}
