using Cofoundry.Core.Validation;
using Cofoundry.Core.Validation.Internal;
using System;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Library of validation errors relating to users.
    /// </summary>
    public static class UserValidationErrors
    {
        /// <summary>
        /// Library of validation errors used in <see cref="AuthenticateUserCredentialsQuery"/> or other
        /// functions that require authentication.
        /// </summary>
        public static class Authentication
        {
            /// <summary>
            /// Either the username or password is invalid.
            /// </summary>
            public static readonly ValidationErrorTemplate InvalidCredentials = new ValidationErrorTemplate(
                AddAuthenticationNamespace("invalid-credentials"),
                "Invalid username or password.",
                e => new InvalidCredentialsAuthenticationException(e)
                );

            /// <summary>
            /// To be used when only authenticating the password for a logged in user 
            /// and authentication fails e.g. when updating a password
            /// </summary>
            public static readonly ValidationErrorTemplate InvalidPassword = new ValidationErrorTemplate(
                AddAuthenticationNamespace("invalid-password"),
                "Incorrect password.",
                e => new InvalidCredentialsAuthenticationException(e)
                );

            /// <summary>
            /// Too many failed authentication attempts have occurred either for the
            /// username or IP address.
            /// </summary>
            public static readonly ValidationErrorTemplate TooManyFailedAttempts = new ValidationErrorTemplate(
                AddAuthenticationNamespace("rate-limit-exceeded"),
                "Too many failed authentication attempts have been detected, please try again later."
                );

            /// <summary>
            /// The error was not specified. This can be used when an error
            /// is picked up outside of the core authentication operation e.g.
            /// in MVC if the ModelState is invalid and the result is returned
            /// before authentication is attempted.
            /// </summary>
            public static readonly ValidationErrorTemplate NotSpecified = new ValidationErrorTemplate(
                AddAuthenticationNamespace("not-specified"),
                "The authentication attempt was unsuccessful."
                );

            /// <summary>
            /// The credentials are valid but a password change is required before sign in is permitted.
            /// This error isn't expected to be shown to the user but is instead expected to be intercepted 
            /// and handled in the UI.
            /// </summary>
            public static readonly ValidationErrorTemplate PasswordChangeRequired = new ValidationErrorTemplate(
                AddAuthenticationNamespace("password-change-required"),
                "A password change is required before you can log in.",
                e => new PasswordChangeRequiredException(e)
                );

            /// <summary>
            /// The credentials are valid but the account has not been verified, and the user area is configured to
            /// not allow sign ins for unverified users.
            /// </summary>
            public static readonly ValidationErrorTemplate AccountNotVerified = new ValidationErrorTemplate(
                AddAuthenticationNamespace("account-not-verified"),
                "You account needs to be verified before you can log in.",
                e => new AccountNotVerifiedException(e)
                );

            private static string AddAuthenticationNamespace(string errorCode)
            {
                return AddNamespace("auth-" + errorCode);
            }
        }

        /// <summary>
        /// Library of validation errors relating to account recovery (AKA forgot password) 
        /// functionality.
        /// </summary>
        public static class AccountRecovery
        {
            /// <summary>
            /// Used to namespace Cofoundry error codes for errors returned
            /// from <see cref="InitiateUserAccountRecoveryViaEmailCommand"/>.
            /// </summary>
            public static class Initiation
            {
                /// <summary>
                /// The configured IP address rate limit has been exceeded.
                /// </summary>
                public static readonly ValidationErrorTemplate RateLimitExceeded = new ValidationErrorTemplate(
                    AddInitiationNamespace("rate-limit-exceeded"),
                    "Maximum password reset attempts exceeded."
                    );

                private static string AddInitiationNamespace(string errorCode)
                {
                    return AddAccountRecoveryNamespace("initiation-" + errorCode);
                }
            }

            /// <summary>
            /// Used to namespace Cofoundry error codes for errors returned
            /// from <see cref="ValidateUserAccountRecoveryByEmailQuery"/>.
            /// </summary>
            public static class RequestValidation
            {
                private static readonly Dictionary<string, Func<ValidationErrorTemplate>> ErrorMap = new Dictionary<string, Func<ValidationErrorTemplate>>()
                {
                    { AuthorizedTaskValidationErrors.TokenValidation.NotFound.ErrorCode , () => NotFound },
                    { AuthorizedTaskValidationErrors.TokenValidation.Invalidated.ErrorCode , () => Invalidated },
                    { AuthorizedTaskValidationErrors.TokenValidation.AlreadyComplete.ErrorCode , () => AlreadyComplete },
                    { AuthorizedTaskValidationErrors.TokenValidation.Expired.ErrorCode , () => Expired },
                };

                /// <summary>
                /// Invalid id and token combination. This can include
                /// situations where the id or token are not correctly
                /// formatted, or if the request cannot be located in the database.
                /// </summary>
                public static readonly ValidationErrorTemplate NotFound = new ValidationErrorTemplate(
                    AddRequestValidationNamespace("not-found"),
                    "The account recovery request is not valid."
                    );

                /// <summary>
                /// The request has been invalidated, likely because the
                /// password has already been updated, or a valid sign in
                /// has occurred.
                /// </summary>
                public static readonly ValidationErrorTemplate Invalidated = new ValidationErrorTemplate(
                    AddRequestValidationNamespace("invalidated"),
                    "The account recovery request is no longer valid."
                    );

                /// <summary>
                /// The request exists but has already been completed.
                /// </summary>
                public static readonly ValidationErrorTemplate AlreadyComplete = new ValidationErrorTemplate(
                    AddRequestValidationNamespace("already-complete"),
                    "The account recovery request has already been completed."
                    );

                /// <summary>
                /// The request exists but has expired.
                /// </summary>
                public static readonly ValidationErrorTemplate Expired = new ValidationErrorTemplate(
                    AddRequestValidationNamespace("expired"),
                    "The account recovery request has expired."
                    );

                public static ValidationError Map(AuthorizedTaskTokenValidationResult result)
                {
                    if (result.IsSuccess) return null;

                    var mappedError = ErrorMap.GetValueOrDefault(result.Error.ErrorCode)?.Invoke();
                    if (mappedError != null)
                    {
                        return mappedError.Create();
                    }

                    return result.Error;
                }

                private static string AddRequestValidationNamespace(string errorCode)
                {
                    return AddAccountRecoveryNamespace("request-" + errorCode);
                }
            }

            private static string AddAccountRecoveryNamespace(string errorCode)
            {
                return AddNamespace("account-recovery-" + errorCode);
            }
        }


        /// <summary>
        /// Library of validation errors relating to account verification (AKA confirm account) 
        /// functionality.
        /// </summary>
        public static class AccountVerification
        {
            /// <summary>
            /// Used to namespace Cofoundry error codes for errors returned
            /// from <see cref="InitiateUserAccountVerificationViaEmailCommand"/>.
            /// </summary>
            public static class Initiation
            {
                /// <summary>
                /// The configured IP address rate limit has been exceeded.
                /// </summary>
                public static readonly ValidationErrorTemplate RateLimitExceeded = new ValidationErrorTemplate(
                    AddInitiationNamespace("rate-limit-exceeded"),
                    "Maximum account verification requests exceeded."
                    );

                /// <summary>
                /// The user account aready contains an AcccountVerificationDate
                /// indicating that it has already been verified.
                /// </summary>
                public static readonly ValidationErrorTemplate AlreadyVerified = new ValidationErrorTemplate(
                    AddInitiationNamespace("already-verified"),
                    "The account is already verified."
                    );

                private static string AddInitiationNamespace(string errorCode)
                {
                    return AddAccountRecoveryNamespace("initiation-" + errorCode);
                }
            }

            /// <summary>
            /// Used to namespace Cofoundry error codes for errors returned
            /// from <see cref="ValidateUserAccountVerificationByEmailQuery"/>.
            /// </summary>
            public static class RequestValidation
            {
                private static readonly Dictionary<string, Func<ValidationErrorTemplate>> ErrorMap = new Dictionary<string, Func<ValidationErrorTemplate>>()
                {
                    { AuthorizedTaskValidationErrors.TokenValidation.NotFound.ErrorCode , () => NotFound },
                    { AuthorizedTaskValidationErrors.TokenValidation.Invalidated.ErrorCode , () => Invalidated },
                    { AuthorizedTaskValidationErrors.TokenValidation.AlreadyComplete.ErrorCode , () => AlreadyComplete },
                    { AuthorizedTaskValidationErrors.TokenValidation.Expired.ErrorCode , () => Expired },
                };

                /// <summary>
                /// Invalid id and token combination. This can include
                /// situations where the id or token are not correctly
                /// formatted, or if the request cannot be located in the database.
                /// </summary>
                public static readonly ValidationErrorTemplate NotFound = new ValidationErrorTemplate(
                    AddRequestValidationNamespace("not-found"),
                    "The account verification request is not valid."
                    );

                /// <summary>
                /// The request has been invalidated, likely because the
                /// account has already been verified by another request.
                /// </summary>
                public static readonly ValidationErrorTemplate Invalidated = new ValidationErrorTemplate(
                    AddRequestValidationNamespace("invalidated"),
                    "The account verification request is no longer valid."
                    );

                /// <summary>
                /// The request exists but has already been completed.
                /// </summary>
                public static readonly ValidationErrorTemplate AlreadyComplete = new ValidationErrorTemplate(
                    AddRequestValidationNamespace("already-complete"),
                    "The account verification request has already been completed."
                    );

                /// <summary>
                /// The request exists but has expired.
                /// </summary>
                public static readonly ValidationErrorTemplate Expired = new ValidationErrorTemplate(
                    AddRequestValidationNamespace("expired"),
                    "The account verification request has expired."
                    );

                /// <summary>
                /// The request exists but the user has updated their email since the request was sent and is 
                /// therefore considered expired.
                /// </summary>
                public static readonly ValidationErrorTemplate EmailMismatch = new ValidationErrorTemplate(
                    AddRequestValidationNamespace("email-mismatch"),
                    "The account verification request has expired."
                    );

                public static ValidationError Map(AuthorizedTaskTokenValidationResult result)
                {
                    if (result.IsSuccess) return null;

                    var mappedError = ErrorMap.GetValueOrDefault(result.Error.ErrorCode)?.Invoke();
                    if (mappedError != null)
                    {
                        return mappedError.Create();
                    }

                    return result.Error;
                }

                private static string AddRequestValidationNamespace(string errorCode)
                {
                    return AddAccountRecoveryNamespace("request-" + errorCode);
                }
            }

            private static string AddAccountRecoveryNamespace(string errorCode)
            {
                return AddNamespace("account-verification-" + errorCode);
            }
        }

        /// <summary>
        /// Library of validation errors used in in email address validation e.g. in 
        /// <see cref="IEmailAddressValidator"/>.
        /// </summary>
        public static class EmailAddress
        {
            /// <summary>
            /// This error occurs if the email address received by the <see cref="IEmailAddressValidator"/>
            /// is <see langword="null"/>, which typically indicaztes that the <see cref="IEmailAddressNormalizer{TUserArea}"/> 
            /// could not parse the email address and did not return a result.
            /// </summary>
            public static readonly ValidationErrorTemplate InvalidFormat = new ValidationErrorTemplate(
                AddEmailAddressNamespace("invalid-format"),
                "Email is in an invalid format."
                );

            /// <summary>
            /// The email is shorter then the limit defined in the 
            /// <see cref="EmailAddressOptions"/> configuration settings.
            /// </summary>
            public static readonly ValidationErrorTemplate MinLengthNotMet = new ValidationErrorTemplate(
                AddEmailAddressNamespace("min-length-not-met"),
                "Email cannot be less than {0} characters."
                );

            /// <summary>
            /// The email is longer then the limit defined in the 
            /// <see cref="EmailAddressOptions"/> configuration settings.
            /// </summary>
            public static readonly ValidationErrorTemplate MaxLengthExceeded = new ValidationErrorTemplate(
                AddEmailAddressNamespace("max-length-exceeded"),
                "Email cannot be more than {0} characters."
                );

            /// <summary>
            /// The email contains characters that are not permitted by the 
            /// <see cref="EmailAddressOptions"/> configuration settings.
            /// </summary>
            public static readonly ValidationErrorTemplate InvalidCharacters = new ValidationErrorTemplate(
                AddEmailAddressNamespace("invalid-characters"),
                "Email cannot contain '{0}'."
                );

            /// <summary>
            /// The email is already registered with another user.
            /// </summary>
            public static readonly ValidationErrorTemplate NotUnique = new ValidationErrorTemplate(
                AddEmailAddressNamespace("not-unique"),
                "This email is already registered."
                );

            private static string AddEmailAddressNamespace(string errorCode)
            {
                return AddNamespace("email-" + errorCode);
            }
        }

        /// <summary>
        /// Library of validation errors used in in username validation e.g. in <see cref="IUsernameValidator"/>.
        /// </summary>
        public static class Username
        {
            /// <summary>
            /// This error occurs if the username received by the <see cref="IUsernameValidator"/>
            /// is <see langword="null"/>. This should rarely happen, only if the username 
            /// contains only characters stripped out through a custom <see cref="IUsernameNormalizer{TUserArea}"/>.
            /// </summary>
            public static readonly ValidationErrorTemplate InvalidFormat = new ValidationErrorTemplate(
                AddEmailAddressNamespace("invalid-format"),
                "Username is in an invalid format."
                );

            /// <summary>
            /// The username is shorter then the limit defined in the 
            /// <see cref="UsernameOptions"/> configuration settings.
            /// </summary>
            public static readonly ValidationErrorTemplate MinLengthNotMet = new ValidationErrorTemplate(
                AddEmailAddressNamespace("min-length-not-met"),
                "Username cannot be less than {0} characters."
                );

            /// <summary>
            /// The username is longer then the limit defined in the 
            /// <see cref="UsernameOptions"/> configuration settings.
            /// </summary>
            public static readonly ValidationErrorTemplate MaxLengthExceeded = new ValidationErrorTemplate(
                AddEmailAddressNamespace("max-length-exceeded"),
                "Username cannot be more than {0} characters."
                );

            /// <summary>
            /// The username contains characters that are not permitted by the 
            /// <see cref="UsernameOptions"/> configuration settings.
            /// </summary>
            public static readonly ValidationErrorTemplate InvalidCharacters = new ValidationErrorTemplate(
                AddEmailAddressNamespace("invalid-characters"),
                "Username cannot contain '{0}'."
                );

            /// <summary>
            /// The username is already registered with another user.
            /// </summary>
            public static readonly ValidationErrorTemplate NotUnique = new ValidationErrorTemplate(
                AddEmailAddressNamespace("not-unique"),
                "This username is already registered."
                );

            private static string AddEmailAddressNamespace(string errorCode)
            {
                return AddNamespace("username-" + errorCode);
            }
        }

        private static string AddNamespace(string errorCode)
        {
            return ValidationErrorCodes.AddNamespace(errorCode, "user");
        }
    }
}