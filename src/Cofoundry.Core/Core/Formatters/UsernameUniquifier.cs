namespace Cofoundry.Core.Extendable;

/// <inheritdoc/>
public class UsernameUniquifier : IUsernameUniquifier
{
    private readonly IUsernameNormalizer _usernameNormalizer;

    public UsernameUniquifier(
        IUsernameNormalizer usernameNormalizer
        )
    {
        _usernameNormalizer = usernameNormalizer;
    }

    public virtual string Uniquify(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;

        var result = _usernameNormalizer
            .Normalize(username)
            .ToLowerInvariant();

        return result;
    }
}
