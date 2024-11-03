using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MailKitExample.Pages
{
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
        public string EmailAddress { get; set; }

        [BindProperty]
        public string DisplayName { get; set; }

        [BindProperty]
        [Required]
        public string Message { get; set; }

        public string Result { get; set; }

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
}
