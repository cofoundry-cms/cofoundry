using Cofoundry.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Exception thrown from the ValidatePasswordResetRequestQueryHandler
    /// when the request is malformed or invalid for exceptional reasons.
    /// </summary>
    public class InvalidPasswordResetRequestException : Exception
    {
        const string MESSAGE = "Invalid password request";

        public InvalidPasswordResetRequestException(ValidatePasswordResetRequestQuery query)
            : this(query, MESSAGE)
        {
        }

        public InvalidPasswordResetRequestException(ValidatePasswordResetRequestQuery query, string message)
            : base(message)
        {
            if (query == null) return;

            UserPasswordResetRequestId = query.UserPasswordResetRequestId;
            Token = query.Token;
            UserAreaCode = query.UserAreaCode;
        }

        public Guid UserPasswordResetRequestId { get; }

        public string Token { get; }

        public string UserAreaCode { get; }
    }
}
