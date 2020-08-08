using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to generate strings of random characters with a customizable
    /// length and alphabet.
    /// </summary>
    public class RandomStringGenerator : IRandomStringGenerator
    {
        private const string DEFAULT_CHARACTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private const string DEFAULT_UNTRUSTWORTHY_CHARACTERS = "cfhistukvCFHISTUKV";

        /// <summary>
        /// Generates a random string of characters at a specific length. This uses
        /// the default alphabet which uses mixed-case alpha numeric characters. The 
        /// generator also defines a set of "untrustworthy" characters which 
        /// helps prevent common curse words in the english language, but you can override 
        /// this by supplying your own set in one of the overloads. The default character set 
        /// is "cfhistukvCFHISTUKV".
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        public string Generate(int length)
        {
            return Generate(length, DEFAULT_CHARACTERS, DEFAULT_UNTRUSTWORTHY_CHARACTERS);
        }

        /// <summary>
        /// Generates a random string of characters at a specific length with a set 
        /// alphabet. The generator also uses a set of "untrustworthy" characters which 
        /// helps prevent common curse words in the english language. The default character 
        /// set is "cfhistukvCFHISTUKV". 
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <param name="allowedCharacters">
        /// The set of characters to use when generating the characters in the string.
        /// Duplicate characters are allows, which will simply increase the likelyhood 
        /// of selecting those characters.
        /// </param>
        public string Generate(int length, string allowedCharacters)
        {
            return Generate(length, allowedCharacters, DEFAULT_UNTRUSTWORTHY_CHARACTERS);
        }

        /// <summary>
        /// Generates a random string of characters at a specific length. You can
        /// optionally define an alphabet, but by default it uses mixed-case alpha
        /// numeric characters. The generator also uses a set of "untrustworthy" 
        /// characters by default which helps prevent common curse words in the
        /// english language, but you can override this by supplying your own set
        /// or null to remove the filter altogether.
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <param name="allowedCharacters">
        /// The set of characters to use when generating the characters in the string.
        /// This default to mixed-case alpha numeric characters. Duplicate characters
        /// are allows, which will simply increase the likelyhood of selecting those 
        /// characters.
        /// </param>
        /// <param name="untrustworthyCharacters">
        /// A set of characters that should not be allowed to be placed next to each 
        /// other, which serves as a crude profanity filter. These characters will appear slightly less often
        /// in general. Passing null or an empty string prevents the filter from being 
        /// run.
        /// </param>
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

            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
            }
            
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
