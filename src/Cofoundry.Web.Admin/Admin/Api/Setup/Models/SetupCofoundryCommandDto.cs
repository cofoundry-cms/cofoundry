using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web.Admin
{
    public class SetupCofoundryCommandDto
    {
        [Required]
        [StringLength(50)]
        public string ApplicationName { get; set; }

        [StringLength(32)]
        public string FirstName { get; set; }

        [StringLength(32)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(PasswordOptions.MAX_LENGTH_BOUNDARY, MinimumLength = PasswordOptions.MIN_LENGTH_BOUNDARY)]
        public string Password { get; set; }
    }
}