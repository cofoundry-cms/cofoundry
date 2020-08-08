using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class RandomStringGeneratorTests
    {
        #region CreateFileStamp

        [Theory]
        [InlineData(1)]
        [InlineData(20)]
        [InlineData(500)]
        public void Generate_WhenPositiveLength_GeneratesCorrectLength(int length)
        {
            var generator = new RandomStringGenerator();

            var result = generator.Generate(length);
            Assert.Equal(length, result.Length);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        public void Generate_WhenNotPositiveLength_ThrowsArgumentException(int length)
        {
            var generator = new RandomStringGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(length));
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("123")]
        [InlineData("$£%")]
        public void Generate_WithAlphabet_DoesNotContainOtherCharacters(string alphabet)
        {
            var generator = new RandomStringGenerator();

            // technically can produce a false positive but very unlikely.
            var result = generator.Generate(500, alphabet, null);
            var containsNonAlphabetCharacter = result.Any(c => !alphabet.Contains(c));
            Assert.False(containsNonAlphabetCharacter);
        }

        [Theory]
        [InlineData("ab", "Z")]
        [InlineData("12", "5")]
        [InlineData("$£", "*")]
        public void Generate_WithUntrustworthCharacters_DoesNotContainNonAlphabetCharacters(string alphabet, string untrustworthyCharacters)
        {
            var generator = new RandomStringGenerator();

            // technically can produce a false positive but very unlikely.
            var result = generator.Generate(500, alphabet, untrustworthyCharacters);
            var containsNonAlphabetCharacter = result.Any(c => !alphabet.Contains(c));
            Assert.False(containsNonAlphabetCharacter);
        }

        [Theory]
        [InlineData("abc", "ab")]
        [InlineData("123", "12")]
        public void Generate_WithUntrustworthCharacters_UntrustworthiesSeparated(string alphabet, string untrustworthyCharacters)
        {
            var generator = new RandomStringGenerator();

            // technically can produce a false positive but very unlikely.
            var result = generator.Generate(1000, alphabet, untrustworthyCharacters);
            var reversed = new string(untrustworthyCharacters.Reverse().ToArray());
            var containsAdjacentUntrustworthyCharacters = result.IndexOf(untrustworthyCharacters) != -1  || result.IndexOf(reversed) != -1;
            Assert.False(containsAdjacentUntrustworthyCharacters);
        }

        [Theory]
        [InlineData("abc", "abc")]
        [InlineData("abcde123456789", "abcde123456789ABCDIEWJ")]
        public void Generate_WhenAlphabetOnlyContainsUntrustworthCharacters_ThrowsArgumentException(string alphabet, string untrustworthyCharacters)
        {
            var generator = new RandomStringGenerator();

            Assert.Throws<ArgumentException>(() => generator.Generate(10, alphabet, untrustworthyCharacters));
        }

        #endregion
    }
}
