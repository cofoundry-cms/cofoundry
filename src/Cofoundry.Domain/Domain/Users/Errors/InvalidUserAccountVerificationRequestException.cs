using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the <see cref="Internal.ValidateUserAccountVerificationByEmailQueryHandler"/>
    /// when the request is invalid for exceptional reasons.
    /// </summary>
    public class InvalidUserAccountVerificationRequestException : Exception
    {
        const string MESSAGE = "Invalid account verification request";

        public InvalidUserAccountVerificationRequestException(ValidateUserAccountVerificationByEmailQuery query)
            : this(query, MESSAGE)
        {
        }

        public InvalidUserAccountVerificationRequestException(ValidateUserAccountVerificationByEmailQuery query, string message)
            : base(message)
        {
            if (query == null) return;

            Token = query.Token;
            UserAreaCode = query.UserAreaCode;
        }

        public string Token { get; }

        public string UserAreaCode { get; }
    }
}
