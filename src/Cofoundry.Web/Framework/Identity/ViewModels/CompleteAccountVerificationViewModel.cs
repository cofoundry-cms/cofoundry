using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web.Identity
{
    /// <inheritdoc/>
    public class CompleteAccountVerificationViewModel : ICompleteAccountVerificationViewModel
    {
        public CompleteAccountVerificationViewModel() { }

        public CompleteAccountVerificationViewModel(string token)
        {
            Token = token;
        }

        [Required]
        public string Token { get; set; }
    }
}