using Cofoundry.Domain.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class PasswordCryptographyService : IPasswordCryptographyService
{
    private readonly IPasswordHasher<PasswordHasherUser> _passwordHasher;

    public PasswordCryptographyService(
        IPasswordHasher<PasswordHasherUser> passwordHasher
        )
    {
        _passwordHasher = passwordHasher;
    }

    public virtual PasswordVerificationResult Verify(string? password, string hash, int hashVersion)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(hash);

        if (string.IsNullOrWhiteSpace(password))
        {
            return PasswordVerificationResult.Failed;
        }

        switch (hashVersion)
        {
            case (int)PasswordHashVersion.V1:
#pragma warning disable CS0618 // For backwards compatibility only
                var isV1Valid = new PasswordCryptographyV1().Verify(password, hash);
#pragma warning restore CS0618 // For backwards compatibility only
                return FormatOldPasswordVersionResult(isV1Valid);
            case (int)PasswordHashVersion.V2:
#pragma warning disable CS0618 // For backwards compatibility only
                var isV2Valid = Defuse.Obsolete.PasswordCryptographyV2.VerifyPassword(password, hash);
#pragma warning restore CS0618 // For backwards compatibility only
                return FormatOldPasswordVersionResult(isV2Valid);
            case (int)PasswordHashVersion.V3:
                var v3Result = _passwordHasher.VerifyHashedPassword(new PasswordHasherUser(), hash, password);
                return v3Result;
            default:
                throw new NotSupportedException("PasswordEncryptionVersion not recognised: " + hashVersion.ToString(CultureInfo.InvariantCulture));
        }
    }

    private static PasswordVerificationResult FormatOldPasswordVersionResult(bool isValid)
    {
        return isValid ? PasswordVerificationResult.SuccessRehashNeeded : PasswordVerificationResult.Failed;
    }

    public virtual PasswordCryptographyResult CreateHash(string password)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(password);

        var result = new PasswordCryptographyResult
        {
            Hash = _passwordHasher.HashPassword(new PasswordHasherUser(), password),
            HashVersion = (int)PasswordHashVersion.V3
        };

        return result;
    }
}
