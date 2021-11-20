using Cofoundry.Domain.CQS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite
{
    public class RegisterMemberAndLogInCommand : ICommand
    {
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(300, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        #region Output

        [OutputValue]
        public int OutputMemberId { get; set; }

        #endregion
    }
}
