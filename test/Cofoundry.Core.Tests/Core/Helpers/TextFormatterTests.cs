using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Core.Tests
{
    public class TextFormatterTests
    {
        #region PascalCaseToSentence

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PascalCaseToSentence_WhenNullOrEmpty_ReturnsEmptyString(string input)
        {
            var result = TextFormatter.PascalCaseToSentence(input);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("    ")]
        public void PascalCaseToSentence_WhenWhitespace_ReturnsWhitespace(string input)
        {
            var result = TextFormatter.PascalCaseToSentence(input);

            Assert.Equal(result, input);
        }

        [Theory]
        [InlineData("ThisAndThat", "This and that")]
        [InlineData("ofMiceAndMen", "of mice and men")]
        public void PascalCaseToSentence_CanDoBasicConversion(string input, string expected)
        {
            var result = TextFormatter.PascalCaseToSentence(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("CostInGBP", "Cost in GBP")]
        [InlineData("WinningWithTheABCMethod", "Winning with the ABC method")]
        public void PascalCaseToSentence_KeepsAcronymsIntact(string input, string expected)
        {
            var result = TextFormatter.PascalCaseToSentence(input);

            Assert.Equal(expected, result);
        }

        #endregion

        #region PascalCaseToSentence

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Camelize_WhenNullOrWhitespace_ReturnsEmptyString(string input)
        {
            var result = TextFormatter.Camelize(input);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData("this-and-that", "thisAndThat")]
        [InlineData("of mice and men", "ofMiceAndMen")]
        [InlineData("under_score-dash", "underScoreDash")]
        public void Camelize_CanDoBasicConversion(string input, string expected)
        {
            var result = TextFormatter.Camelize(input);

            Assert.Equal(expected, result);
        }

        #endregion

        #region PascalCaseToSentence

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Pascalize_WhenNullOrWhitespace_ReturnsEmptyString(string input)
        {
            var result = TextFormatter.Pascalize(input);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData("  space needs trimming  ", "SpaceNeedsTrimming")]
        [InlineData("this-and-that", "ThisAndThat")]
        [InlineData("of mice and men", "OfMiceAndMen")]
        [InlineData("under_score-dash", "UnderScoreDash")]
        public void Pascalize_CanDoBasicConversion(string input, string expected)
        {
            var result = TextFormatter.Pascalize(input);

            Assert.Equal(expected, result);
        }

        #endregion

        #region LimitWithElipses

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Limit_WhenNullOrEmpty_ReturnsEmptyString(string input)
        {
            var result = TextFormatter.Limit(input, 10);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("    ")]
        public void Limit_WhenWhitespace_ReturnsWhitespace(string input)
        {
            var result = TextFormatter.Limit(input, 10);

            Assert.Equal(result, input);
        }

        [Theory]
        [InlineData("I have not reached the limit", 28, "I have not reached the limit")]
        [InlineData("I really have not reached the limit", 100, "I really have not reached the limit")]
        public void Limit_WhenUnderLimit_NotLimited(string input, int characterCount, string expected)
        {
            var result = TextFormatter.Limit(input, characterCount);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("I have so reached the limit", 26, "I have so reached the limi")]
        [InlineData("I have so reached the limit", 10, "I have so")]
        [InlineData("I have so reached the limit", 11, "I have so r")]
        [InlineData("I have so reached the limit", 9, "I have so")]
        public void Limit_WhenOverLimit_IsLimited(string input, int characterCount, string expected)
        {
            var result = TextFormatter.Limit(input, characterCount);

            Assert.Equal(expected, result);
        }

        #endregion

        #region LimitWithElipses

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void LimitWithElipses_WhenNullOrEmpty_ReturnsEmptyString(string input)
        {
            var result = TextFormatter.LimitWithElipses(input, 10);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("    ")]
        public void LimitWithElipses_WhenWhitespace_ReturnsWhitespace(string input)
        {
            var result = TextFormatter.LimitWithElipses(input, 10);

            Assert.Equal(result, input);
        }

        [Theory]
        [InlineData("I have not reached the limit", 28, "I have not reached the limit")]
        [InlineData("I really have not reached the limit", 100, "I really have not reached the limit")]
        public void LimitWithElipses_WhenUnderLimit_NotLimited(string input, int characterCount, string expected)
        {
            var result = TextFormatter.LimitWithElipses(input, characterCount);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("I have so reached the limit", 26, "I have so reached the lim…")]
        [InlineData("I have so reached the limit", 10, "I have so…")]
        [InlineData("I have so reached the limit", 11, "I have so…")]
        [InlineData("I have so reached the limit", 9, "I have s…")]
        public void LimitWithElipses_WhenOverLimit_IsLimited(string input, int characterCount, string expected)
        {
            var result = TextFormatter.LimitWithElipses(input, characterCount);

            Assert.Equal(expected, result);
        }

        #endregion

        #region LimitWithElipsesOnWordBoundary

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void LimitWithElipsesOnWordBoundary_WhenNullOrEmpty_ReturnsEmptyString(string input)
        {
            var result = TextFormatter.LimitWithElipsesOnWordBoundary(input, 10);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("    ")]
        public void LimitWithElipsesOnWordBoundary_WhenWhitespace_ReturnsWhitespace(string input)
        {
            var result = TextFormatter.LimitWithElipsesOnWordBoundary(input, 10);

            Assert.Equal(result, input);
        }

        [Theory]
        [InlineData("I have not reached the limit", 28, "I have not reached the limit")]
        [InlineData("I really have not reached the limit", 100, "I really have not reached the limit")]
        public void LimitWithElipsesOnWordBoundary_WhenUnderLimit_NotLimited(string input, int characterCount, string expected)
        {
            var result = TextFormatter.LimitWithElipsesOnWordBoundary(input, characterCount);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("I have so reached the limit", 26, "I have so reached the…")]
        [InlineData("I have so reached the limit", 10, "I have so…")]
        [InlineData("I have so reached the limit", 11, "I have so…")]
        [InlineData("I have so reached the limit", 9, "I have…")]
        public void LimitWithElipsesOnWordBoundary_WhenOverLimit_IsLimited(string input, int characterCount, string expected)
        {
            var result = TextFormatter.LimitWithElipsesOnWordBoundary(input, characterCount);

            Assert.Equal(expected, result);
        }

        #endregion

        #region FirstLetterToUpperCase

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FirstLetterToUpperCase_WhenNullOrEmpty_ReturnsEmptyString(string input)
        {
            var result = TextFormatter.FirstLetterToUpperCase(input);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("    ")]
        public void FirstLetterToUpperCase_WhenWhitespace_ReturnsWhitespace(string input)
        {
            var result = TextFormatter.FirstLetterToUpperCase(input);

            Assert.Equal(result, input);
        }

        [Theory]
        [InlineData("I'm already upper case", "I'm already upper case")]
        [InlineData("i'm not lower cased", "I'm not lower cased")]
        [InlineData("i have a MIX of Lower and uppR case", "I have a MIX of Lower and uppR case")]
        public void FirstLetterToUpperCase_CanConvert(string input, string expected)
        {
            var result = TextFormatter.FirstLetterToUpperCase(input);

            Assert.Equal(expected, result);
        }

        #endregion

        #region RemoveDiacritics

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RemoveDiacritics_WhenNullOrEmpty_ReturnsEmptyString(string input)
        {
            var result = TextFormatter.PascalCaseToSentence(input);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("    ")]
        public void RemoveDiacritics_WhenWhitespace_ReturnsWhitespace(string input)
        {
            var result = TextFormatter.PascalCaseToSentence(input);

            Assert.Equal(result, input);
        }

        [Theory]
        [InlineData("Crème brûlée", "Creme brulee")]
        [InlineData("diakrī́nō", "diakrino")]
        [InlineData("Что-то хорошее", "Chto-to khoroshee")]
        [InlineData("It's some text, without-diacritics/but with punctuation!", "It's some text, without-diacritics/but with punctuation!")]
        public void RemoveDiacritics_CanRemoveDiacritics(string input, string expected)
        {
            var result = TextFormatter.RemoveDiacritics(input);

            Assert.Equal(expected, result);
        }

        #endregion
    }
}
