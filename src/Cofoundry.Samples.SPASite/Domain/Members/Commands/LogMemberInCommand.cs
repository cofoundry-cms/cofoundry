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
    public class LogMemberInCommand : ICommand
    {
        [Required]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
