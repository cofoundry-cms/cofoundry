using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to generate strings of random characters with a customizable
    /// length and alphabet.
    /// </summary>
    public interface IRandomStringGenerator
    {
        /// <summary>
        /// Generates a random string of characters at a specific length. This uses
        /// the default alphabet which uses mixed-case alpha numeric characters. The 
        /// generator also defines a set of "untrustworthy" characters which 
        /// helps prevent common curse words in the english language, but you can override 
        /// this by supplying your own set in one of the overloads. The untrustworthy character 
        /// set in the default implementation is "cfhistukvCFHISTUKV".
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        string Generate(int length);

        /// <summary>
        /// Generates a random string of characters at a specific length with a set 
        /// alphabet. The generator also uses a set of "untrustworthy" characters which 
        /// helps prevent common curse words in the english language. The untrustworthy 
        /// character set in the default implementation is "cfhistukvCFHISTUKV".
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <param name="allowedCharacters">
        /// The set of characters to use when generating the characters in the string.
        /// Duplicate characters are allows, which will simply increase the likelyhood 
        /// of selecting those characters.
        /// </param>
        string Generate(int length, string allowedCharacters);

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
        /// other, which serves as a crude profanity filter. These characters will appear 
        /// slightly less often in general. Passing null or an empty string prevents the 
        /// filter from being run.
        /// </param>
        string Generate(int length, string allowedCharacters, string untrustworthyCharacters);
    }
}
