using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Resets a users password to a randomly generated temporary value
    /// and sends it in a mail a notification to the user. The password
    /// will need to be changed at first login (if the user area supports 
    /// it). This is designed to be used from an admin screen rather than 
    /// a self-service reset which can be done via 
    /// InitiatePasswordResetRequestCommand.
    /// </summary>
    public class ResetUserPasswordCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Required. The database id of the user to reset the password to.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int UserId { get; set; }
    }
}
