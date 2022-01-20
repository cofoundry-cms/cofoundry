using Cofoundry.Core;
using System;

namespace Cofoundry.Domain
{
    /// <inheritdoc/>
    public class UserAccountRecoveryTokenFormatter : IUserAccountRecoveryTokenFormatter
    {
        public string Format(UserAccountRecoveryTokenParts parts)
        {
            if (parts == null) throw new ArgumentNullException(nameof(parts));
            if (parts.UserAccountRecoveryRequestId == Guid.Empty)
            {
                throw new ArgumentNullException("parts.UserAccountRecoveryRequestId");
            }
            if (string.IsNullOrWhiteSpace(parts.AuthorizationCode))
            {
                throw new ArgumentEmptyException("parts.AuthorizationCode");
            }

            return parts.UserAccountRecoveryRequestId.ToString("N") + "-" + parts.AuthorizationCode;
        }

        public UserAccountRecoveryTokenParts Parse(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            token = token.Trim();

            var splitIndex = token.IndexOf("-");
            if (splitIndex < 32) return null;

            var idPart = token.Remove(splitIndex);
            if (!Guid.TryParse(idPart, out var id)) return null;

            var authCode = token.Substring(splitIndex + 1);
            if (string.IsNullOrWhiteSpace(authCode)) return null;

            return new UserAccountRecoveryTokenParts()
            {
                AuthorizationCode = authCode,
                UserAccountRecoveryRequestId = id
            };
        }
    }
}
