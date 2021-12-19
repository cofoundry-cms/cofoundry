﻿using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the user account of the currently logged in user.
    /// </summary>
    public class UpdateCurrentUserAccountCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// The first name is optional.
        /// </summary>
        [StringLength(32)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name is optional.
        /// </summary>
        [StringLength(32)]
        public string LastName { get; set; }

        private string _email = null;
        /// <summary>
        /// The email address is required if the user area has 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> set to <see langword="true"/>.
        /// </summary>
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        public string Email
        {
            get { return _email; }
            set { _email = value == string.Empty ? null : value; }
        }

        /// <summary>
        /// The username is required if the user area has UseEmailAsUsername set to 
        /// false, otherwise it should be empty and the Email address will be used 
        /// as the username instead.
        /// </summary>
        [StringLength(150)]
        public string Username { get; set; }
    }
}
