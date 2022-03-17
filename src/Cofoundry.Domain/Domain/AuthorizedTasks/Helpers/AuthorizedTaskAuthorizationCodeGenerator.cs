using Cofoundry.Core.Internal;
using System.Security.Cryptography;

namespace Cofoundry.Domain.Internal;

public class AuthorizedTaskAuthorizationCodeGenerator : IAuthorizedTaskAuthorizationCodeGenerator
{
    public string Generate()
    {
        var bytes = new byte[32];

        RandomNumberGenerator.Fill(bytes);
        var token = Base32Converter
            .ToBase32(bytes)
            .TrimEnd('=')
            .ToLowerInvariant();
        return token;
    }
}
