using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A specialist form of NotPermittedException giving more information 
    /// about the user and the IPermissionApplication that failed the check
    /// </summary>
    public class PermissionValidationFailedException : NotPermittedException
    {
        public PermissionValidationFailedException() : base() { }
            
        public PermissionValidationFailedException(
            IPermissionApplication permission, 
            IUserContext userContext
            )
            : base(FormatMessage(permission, userContext))
        {
            Permission = permission;
            UserContext = userContext;
        }

        /// <summary>
        /// The permission that the user did not have that caused the exception
        /// </summary>
        public IPermissionApplication Permission { get; private set; }

        /// <summary>
        /// Information about the user that failed the pewrmission check
        /// </summary>
        public IUserContext UserContext { get; private set; }

        private static string FormatMessage(
            IPermissionApplication permission,
            IUserContext userContext
            )
        {
            return $"Permission Validation Check Failed. Permission Type: { permission?.ToString() }. UserId: { userContext?.UserId }";
        }

    }
}
