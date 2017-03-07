using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the password of the currently logged in user, using the
    /// OldPassword field to authenticate the request.
    /// </summary>
    public class UpdateCurrentUserUserPasswordCommand : ICommand, ILoggableCommand
    {
        [Required]
        [DataType(DataType.Password)]
        [XmlIgnore]
        [JsonIgnore]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [XmlIgnore]
        [JsonIgnore]
        public string NewPassword { get; set; }
    }
}
