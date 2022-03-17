using Cofoundry.Core.Validation.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Library of validation errors relating to authorize tasks.
/// </summary>
public static class AuthorizedTaskValidationErrors
{
    /// <summary>
    /// Used to namespace Cofoundry error codes for errors returned
    /// from <see cref="AddAuthorizedTaskCommand"/>.
    /// </summary>
    public static class Create
    {
        /// <summary>
        /// If rate limiting is specified, then this is the error that will occur
        /// if that rate limit is exceeded.
        /// </summary>
        public static readonly ValidationErrorTemplate RateLimitExceeded = new ValidationErrorTemplate(
            AddTokenGenerationNamespace("rate-limit-exceeded"),
            "The maximum permitted requests has been exceeded."
            );

        private static string AddTokenGenerationNamespace(string errorCode)
        {
            return AddNamespace("create-" + errorCode);
        }
    }

    /// <summary>
    /// Used to namespace Cofoundry error codes for errors returned
    /// from <see cref="ValidateAuthorizedTaskTokenQuery"/>.
    /// </summary>
    public static class TokenValidation
    {
        /// <summary>
        /// Invalid id and token combination. This can include
        /// situations where the id or token are not correctly
        /// formatted, or if the task cannot be located in the database.
        /// </summary>
        public static readonly ValidationErrorTemplate NotFound = new ValidationErrorTemplate(
            AddRequestValidationNamespace("not-found"),
            "The token is not valid."
            );

        /// <summary>
        /// The task has been invalidated by another action, such as another
        /// task performing the same action, or the action having been completed
        /// through a separate route.
        /// </summary>
        public static readonly ValidationErrorTemplate Invalidated = new ValidationErrorTemplate(
            AddRequestValidationNamespace("invalidated"),
            "The token is no longer valid."
            );

        /// <summary>
        /// The task exists but has already been completed.
        /// </summary>
        public static readonly ValidationErrorTemplate AlreadyComplete = new ValidationErrorTemplate(
            AddRequestValidationNamespace("already-complete"),
            "The request has already been completed."
            );

        /// <summary>
        /// The task exists but has expired.
        /// </summary>
        public static readonly ValidationErrorTemplate Expired = new ValidationErrorTemplate(
            AddRequestValidationNamespace("expired"),
            "The token has expired."
            );

        /// <summary>
        /// Removes the namespace part of the error code to make it easier to
        /// deal with when you are dealing with token validation error codes onlu.
        /// </summary>
        /// <param name="errorCode">
        /// The error code to remove the namespace on.
        /// </param>
        public static string RemoveNamespace(string errorCode)
        {
            if (errorCode == null) throw new ArgumentNullException(nameof(errorCode));

            var ns = AddRequestValidationNamespace(string.Empty);

            if (!errorCode.StartsWith(ns)) return errorCode;

            return errorCode.Remove(0, ns.Length);
        }

        private static string AddRequestValidationNamespace(string errorCode)
        {
            return AddNamespace("token-validation-" + errorCode);
        }
    }

    private static string AddNamespace(string errorCode)
    {
        return ValidationErrorCodes.AddNamespace(errorCode, "authorized-tasks");
    }
}
