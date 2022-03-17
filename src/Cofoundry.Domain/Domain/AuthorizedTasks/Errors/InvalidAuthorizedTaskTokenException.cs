namespace Cofoundry.Domain;

/// <summary>
/// Exception thrown from the <see cref="Internal.ValidateAuthorizedTaskTokenQueryHandler"/>
/// when the token is invalid for exceptional reasons.
/// </summary>
public class InvalidAuthorizedTaskTokenException : Exception
{
    const string MESSAGE = "Invalid authorized task token";

    public InvalidAuthorizedTaskTokenException(ValidateAuthorizedTaskTokenQuery query)
        : this(query, MESSAGE)
    {
    }

    public InvalidAuthorizedTaskTokenException(ValidateAuthorizedTaskTokenQuery query, string message)
        : base(message)
    {
        if (query == null) return;

        Token = query.Token;
        AuthorizedTaskTypeCode = query.AuthorizedTaskTypeCode;
    }

    public string Token { get; }

    public string AuthorizedTaskTypeCode { get; }
}
