using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <remarks>
    /// Sealed because we should be setting these properties
    /// explicitly and shouldn't allow any possible injection of passwords or
    /// user areas.
    /// </remarks>
    public sealed class AddMasterCofoundryUserCommand : ICommand, ILoggableCommand
    {
        [Required]
        [StringLength(32)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(32)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 8)]
        [XmlIgnore]
        [JsonIgnore]
        public string Password { get; set; }

        #region Output

        [OutputValue]
        public int OutputUserId { get; set; }

        #endregion
    }
}
