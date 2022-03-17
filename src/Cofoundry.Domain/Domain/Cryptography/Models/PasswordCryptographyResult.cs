namespace Cofoundry.Domain;

public class PasswordCryptographyResult
{
    public int HashVersion { get; set; }

    public string Hash { get; set; }
}
