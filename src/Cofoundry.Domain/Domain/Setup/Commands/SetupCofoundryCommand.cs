using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    public class SetupCofoundryCommand : ICommand
    {
        [Required]
        [StringLength(50)]
        public string ApplicationName { get; set; }

        [Required]
        [StringLength(32)]
        public string UserFirstName { get; set; }

        [Required]
        [StringLength(32)]
        public string UserLastName { get; set; }

        [Required]
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 8)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string UserPassword { get; set; }
    }
}
