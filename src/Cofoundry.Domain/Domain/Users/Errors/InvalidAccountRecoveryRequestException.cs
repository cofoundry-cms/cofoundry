using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the <see cref="Internal.ValidateUserAccountRecoveryRequestQueryHandler"/>
    /// when the request is invalid for exceptional reasons.
    /// </summary>
    public class InvalidAccountRecoveryRequestException : Exception
    {
        const string MESSAGE = "Invalid password request";

        public InvalidAccountRecoveryRequestException(ValidateUserAccountRecoveryRequestQuery query)
            : this(query, MESSAGE)
        {
        }

        public InvalidAccountRecoveryRequestException(ValidateUserAccountRecoveryRequestQuery query, string message)
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
