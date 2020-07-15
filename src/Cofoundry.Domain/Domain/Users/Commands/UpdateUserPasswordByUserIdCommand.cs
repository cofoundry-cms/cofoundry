using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;
using System.Runtime.Serialization;

namespace Cofoundry.Domain
{
    public class UpdateUserPasswordByUserIdCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [IgnoreDataMember]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string NewPassword { get; set; }
    }
}
