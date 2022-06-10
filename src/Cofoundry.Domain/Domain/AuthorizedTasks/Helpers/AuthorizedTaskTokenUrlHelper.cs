using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class AuthorizedTaskTokenUrlHelper : IAuthorizedTaskTokenUrlHelper
{
    private const string TOKEN_PARAM_NAME = "t";

    public string MakeUrl(
        string baseUri,
        string token
        )
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(baseUri);
        ArgumentEmptyException.ThrowIfNullOrWhitespace(token);

        if (!Uri.IsWellFormedUriString(baseUri, UriKind.RelativeOrAbsolute))
        {
            throw new ArgumentException("Url is not a well formatted url: " + baseUri, nameof(baseUri));
        }

        var queryParams = new Dictionary<string, string>()
        {
            { TOKEN_PARAM_NAME, token }
        };

        var formattedUri = baseUri.ToString().TrimEnd('/');

        var url = QueryHelpers.AddQueryString(formattedUri, queryParams);

        return url;
    }

    public string ParseTokenFromQuery(IQueryCollection queryCollection)
    {
        if (queryCollection.ContainsKey(TOKEN_PARAM_NAME))
        {
            var result = Uri.UnescapeDataString(queryCollection[TOKEN_PARAM_NAME]);

            if (!string.IsNullOrWhiteSpace(result))
            {
                return result;
            }
        }

        return null;
    }
}
