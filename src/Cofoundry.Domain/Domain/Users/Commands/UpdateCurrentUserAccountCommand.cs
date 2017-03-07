using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the user account of the currently logged in user.
    /// </summary>
    public class UpdateCurrentUserAccountCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The first name is required.
        /// </summary>
        [Required]
        [StringLength(32)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name is required.
        /// </summary>
        [Required]
        [StringLength(32)]
        public string LastName { get; set; }

        /// <summary>
        /// The email address is required if the user area has UseEmailAsUsername 
        /// set to true.
        /// </summary>
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        public string Email { get; set; }
    }
}
