using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class UpdateUserCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }

        [Required]
        [StringLength(32)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(32)]
        public string LastName { get; set; }

        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        public string Email { get; set; }

        [StringLength(150)]
        public string Username { get; set; }

        [Required]
        [PositiveInteger]
        public int RoleId { get; set; }

        public bool RequirePasswordChange { get; set; }
    }
}
