using Cofoundry.Core;
using Cofoundry.Core.Validation.Internal;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to namespace Cofoundry error codes for errors relating to
    /// account recovery (AKA forgot password) functionality e.g. "cf-account-recovery-request-expired".
    /// </summary>
    public static class AccountRecoveryErrorCodes
    {
        /// <summary>
        /// Used to namespace Cofoundry error codes for errors returned
        /// from <see cref="InitiateUserAccountRecoveryCommand"/>.
        /// </summary>
        public static class Initiation
        {
            /// <summary>
            /// Invalid id and token combination. This can include
            /// situations where the id or token are not correctly
            /// formatted, or if the request cannot be located in the database.
            /// </summary>
            public static string MaxAttemptsExceeded = AddNamespace("initiation-max-attempts-exceeded");
        }

        /// <summary>
        /// Used to namespace Cofoundry error codes for errors returned
        /// from <see cref="ValidateUserAccountRecoveryRequestQuery"/>.
        /// </summary>
        public static class RequestValidation
        {
            /// <summary>
            /// Invalid id and token combination. This can include
            /// situations where the id or token are not correctly
            /// formatted, or if the request cannot be located in the database.
            /// </summary>
            public static string NotFound = AddNamespace("not-found");

            /// <summary>
            /// The request has been invalidated, likely because the
            /// password has already been updated, or a valid login
            /// has occured.
            /// </summary>
            public static string Invalidated = AddNamespace("invalidated");

            /// <summary>
            /// The request exists but has already been completed.
            /// </summary>
            public static string AlreadyComplete = AddNamespace("already-complete");

            /// <summary>
            /// The request exists but has expired.
            /// </summary>
            public static string Expired = AddNamespace("expired");

            private static string AddNamespace(string errorCode)
            {
                if (string.IsNullOrWhiteSpace(errorCode)) throw new ArgumentEmptyException();

                return AccountRecoveryErrorCodes.AddNamespace("request-" + errorCode);
            }
        }

        private static string AddNamespace(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode)) throw new ArgumentEmptyException();

            return ValidationErrorCodes.AddNamespace(errorCode, "account-recovery");
        }
    }
}