namespace Cofoundry.Domain
{
    public class EmailAddressOptions
    {
        public bool AllowAnyLetters { get; set; } = true;

        public bool AllowAnyDigit { get; set; } = true;

        /// <summary>
        /// TODO: check https://en.wikipedia.org/wiki/Email_address#Examples
        /// https://stackoverflow.com/questions/2049502/what-characters-are-allowed-in-an-email-address
        /// </summary>
        public string AdditionalAllowedCharacters { get; set; } = "!#$%&'*+-/=?^_`{|}~.\"(),:;<>@[\\] ";

        public bool RequireUnique { get; set; }

        public EmailAddressOptions Clone()
        {
            return new EmailAddressOptions()
            {
                AllowAnyLetters = AllowAnyLetters,
                AllowAnyDigit = AllowAnyDigit,
                AdditionalAllowedCharacters = AdditionalAllowedCharacters,
                RequireUnique = RequireUnique
            };
        }
    }
}
