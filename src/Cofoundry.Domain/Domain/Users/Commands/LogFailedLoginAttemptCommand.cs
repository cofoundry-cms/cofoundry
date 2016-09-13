using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class LogFailedLoginAttemptCommand : ICommand
    {
        public LogFailedLoginAttemptCommand()
        {

        }

        public LogFailedLoginAttemptCommand(string userAreaCode, string username)
        {
            Username = username;
            UserAreaCode = userAreaCode;
        }

        [Required]
        public string UserAreaCode { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
