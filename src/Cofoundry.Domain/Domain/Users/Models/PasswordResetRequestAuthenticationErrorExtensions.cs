using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class PasswordResetRequestAuthenticationErrorExtensions
    {
        /// <summary>
        /// Returns a text a basic text representation of the enum for 
        /// displaying as a user friendly error message. Often you'll want
        /// to customize the error messages produced, but this helper can
        /// be used to cut down on boiletplate code if you are creating
        /// a simple implementation.
        /// </summary>
        public static string ToDisplayText(this PasswordResetRequestAuthenticationError error)
        {
            switch (error)
            {
                case PasswordResetRequestAuthenticationError.AlreadyComplete:
                    return "The password recovery request has already been completed.";
                case PasswordResetRequestAuthenticationError.Expired:
                    return "The password recovery request has expired.";
                case PasswordResetRequestAuthenticationError.InvalidRequest:
                    return "The password recovery request is not valid.";
                case PasswordResetRequestAuthenticationError.None:
                    return string.Empty;
                default:
                    throw new Exception($"Unknown {nameof(PasswordResetRequestAuthenticationError)} value: {error}");
            }
        }
    }
}
