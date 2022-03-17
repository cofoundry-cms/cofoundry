using Cofoundry.Domain.Cryptography;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Identity;

namespace Cofoundry.Domain.Tests;

public class PasswordCryptographyServiceTests
{
    const string PASSWORD = "mr goodbytes";

    private PasswordCryptographyService _passwordCryptographyService;

    public PasswordCryptographyServiceTests()
    {
        var identityHasher = new PasswordHasher<PasswordHasherUser>();
        _passwordCryptographyService = new PasswordCryptographyService(identityHasher);
    }

    [Theory]
    [InlineData(PasswordHashVersion.V1)]
    [InlineData(PasswordHashVersion.V2)]
    [InlineData(PasswordHashVersion.V3)]
    public void Verify_WhenPasswordIncorrect_ReturnsFailed(PasswordHashVersion passwordHashVersion)
    {
        var hash = HashPasswordWithVersion(PASSWORD, passwordHashVersion);
        var verificationResult = _passwordCryptographyService.Verify("access main program", hash, (int)passwordHashVersion);

        Assert.Equal(PasswordVerificationResult.Failed, verificationResult);
    }

    [Fact]
    public void Verify_WhenPasswordCorrect_ReturnsSuccess()
    {
        var hashResult = _passwordCryptographyService.CreateHash(PASSWORD);
        var verificationResult = _passwordCryptographyService.Verify(PASSWORD, hashResult.Hash, (int)PasswordHashVersion.V3);

        Assert.Equal(PasswordVerificationResult.Success, verificationResult);
    }

    [Theory]
    [InlineData(PasswordHashVersion.V1)]
    [InlineData(PasswordHashVersion.V2)]
    public void Verify_WhenPasswordOldVersion_ReturnsSuccessRehashNeeded(PasswordHashVersion passwordHashVersion)
    {
        var hash = HashPasswordWithVersion(PASSWORD, passwordHashVersion);
        var verificationResult = _passwordCryptographyService.Verify(PASSWORD, hash, (int)passwordHashVersion);

        Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, verificationResult);
    }

    /// <summary>
    /// Hashes a password with a specific version of the password hasher so we can check 
    /// backwards compatability.
    /// </summary>
    private string HashPasswordWithVersion(string password, PasswordHashVersion passwordHashVersion)
    {
        switch (passwordHashVersion)
        {
            case PasswordHashVersion.V1:
                return new PasswordCryptographyV1().CreateHash(password);
            case PasswordHashVersion.V2:
                return Defuse.PasswordCryptographyV2.CreateHash(password);
            case PasswordHashVersion.V3:
                return new PasswordHasher<PasswordHasherUser>().HashPassword(new PasswordHasherUser(), password);
            default:
                throw new NotSupportedException("PasswordEncryptionVersion not recognised: " + passwordHashVersion.ToString());
        }
    }
}
