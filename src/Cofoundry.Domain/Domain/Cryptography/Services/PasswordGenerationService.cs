namespace Cofoundry.Domain.Internal;

public class PasswordGenerationService : IPasswordGenerationService
{
    private const string ALLOWED_CHARACTERS = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ2345679";

    private const int DEFAULT_LENGTH = 10;

    public string Generate()
    {
        return Generate(DEFAULT_LENGTH);
    }

    public string Generate(int passwordLength)
    {
        if (passwordLength < 6) throw new ArgumentOutOfRangeException(nameof(passwordLength), "Password length cannot be less than 6 characters");
        var generator = new RandomStringGenerator();

        return generator.Generate(passwordLength, ALLOWED_CHARACTERS);
    }
}
