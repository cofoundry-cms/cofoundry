namespace Cofoundry.Core.Extendable
{
    /// <inheritdoc/>
    public class UsernameNormalizer : IUsernameNormalizer
    {
        public virtual string Normalize(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            var normalized = username.Trim();

            return normalized;
        }
    }
}
