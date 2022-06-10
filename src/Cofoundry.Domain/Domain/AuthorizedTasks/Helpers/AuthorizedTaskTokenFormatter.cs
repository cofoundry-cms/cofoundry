namespace Cofoundry.Domain;

/// <inheritdoc/>
public class AuthorizedTaskTokenFormatter : IAuthorizedTaskTokenFormatter
{
    public string Format(AuthorizedTaskTokenParts parts)
    {
        ArgumentNullException.ThrowIfNull(parts);
        ArgumentEmptyException.ThrowIfDefault(parts.AuthorizedTaskId);
        ArgumentEmptyException.ThrowIfNullOrWhitespace(parts.AuthorizationCode);


        return parts.AuthorizedTaskId.ToString("N") + "-" + parts.AuthorizationCode;
    }

    public AuthorizedTaskTokenParts Parse(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        token = token.Trim();

        var splitIndex = token.IndexOf("-");
        if (splitIndex < 32) return null;

        var idPart = token.Remove(splitIndex);
        if (!Guid.TryParse(idPart, out var id)) return null;

        var authCode = token.Substring(splitIndex + 1);
        if (string.IsNullOrWhiteSpace(authCode)) return null;

        return new AuthorizedTaskTokenParts()
        {
            AuthorizationCode = authCode,
            AuthorizedTaskId = id
        };
    }
}
