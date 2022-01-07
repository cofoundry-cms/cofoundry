using Cofoundry.Domain.CQS;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Samples.SPASite
{
    public class RegisterMemberAndLogInCommand : ICommand
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [OutputValue]
        public int OutputMemberId { get; set; }
    }
}
