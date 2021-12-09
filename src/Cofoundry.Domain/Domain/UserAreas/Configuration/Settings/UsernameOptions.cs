namespace Cofoundry.Domain
{
    public class UsernameOptions
    {
        public bool AllowAnyLetters { get; set; } = true;

        public bool AllowAnyDigit { get; set; } = true;

        public string AdditionalAllowedCharacters { get; set; } = "-._'";

        public UsernameOptions Clone()
        {
            return new UsernameOptions()
            {
                AllowAnyLetters = AllowAnyLetters,
                AllowAnyDigit = AllowAnyDigit,
                AdditionalAllowedCharacters = AdditionalAllowedCharacters
            };
        }
    }
}
