using System;
using System.Linq;
using System.Security.Cryptography;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class RandomStringGenerator : IRandomStringGenerator
    {
        private const string DEFAULT_CHARACTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private const string DEFAULT_UNTRUSTWORTHY_CHARACTERS = "cfhistukvCFHISTUKV";

        public string Generate(int length)
        {
            return Generate(length, DEFAULT_CHARACTERS, DEFAULT_UNTRUSTWORTHY_CHARACTERS);
        }

        public string Generate(int length, string allowedCharacters)
        {
            return Generate(length, allowedCharacters, DEFAULT_UNTRUSTWORTHY_CHARACTERS);
        }

        /// <remarks>
        /// The profanity filtering idea is taken from https://hashids.org/, although
        /// the implementation is not the same.
        /// </remarks>
        public string Generate(int length, string allowedCharacters, string untrustworthyCharacters)
        {
            if (length < 1) throw new ArgumentOutOfRangeException($"{nameof(length)} must be a positive integer.", nameof(length));
            if (allowedCharacters == null) throw new ArgumentNullException(nameof(allowedCharacters));

            if (untrustworthyCharacters == null) untrustworthyCharacters = string.Empty;

            var randomBytes = new Byte[length];
            var generatedCharacters = new char[length];
            char? previousCharacter = null;
            var safeCharacterSet = allowedCharacters
                .Where(c => !untrustworthyCharacters.Contains(c))
                .ToArray();

            if (safeCharacterSet.Length == 0)
            {
                throw new ArgumentException($"The {allowedCharacters} argument does not contain any characters that are not in the  {nameof(untrustworthyCharacters)} set ({untrustworthyCharacters})", nameof(allowedCharacters));
            }

            RandomNumberGenerator.Fill(randomBytes);

            for (int i = 0; i < length; i++)
            {
                if (previousCharacter.HasValue && untrustworthyCharacters.Contains(previousCharacter.Value))
                {
                    // use the safe set
                    generatedCharacters[i] = safeCharacterSet[(int)randomBytes[i] % safeCharacterSet.Length];
                }
                else
                {
                    generatedCharacters[i] = allowedCharacters[(int)randomBytes[i] % allowedCharacters.Length];
                }

                previousCharacter = generatedCharacters[i];
            }

            return new string(generatedCharacters);
        }
    }
}
