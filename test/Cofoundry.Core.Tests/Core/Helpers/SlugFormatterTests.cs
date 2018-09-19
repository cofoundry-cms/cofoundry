using System;
using Xunit;

namespace Cofoundry.Core.Tests
{
    public class SlugFormatterTests
    {
        #region ToSlug

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void ToSlug_WhenNullOrWhitespace_ReturnsEmptyString(string s)
        {
            var result = SlugFormatter.ToSlug(s);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData("test-slug")]
        [InlineData("this-that-other")]
        [InlineData("with-numbers-2-1")]
        public void ToSlug_WhenSlug_ReturnsSame(string s)
        {
            var result = SlugFormatter.ToSlug(s);

            Assert.Equal(s, result);
        }

        [Theory]
        [InlineData("Crème brûlée", "creme-brulee")]
        [InlineData("diakrī́nō", "diakrino")]
        [InlineData("Что-то хорошее", "chto-to-khoroshee")]
        public void ToSlug_CanRemoveDiacritics(string input, string expected)
        {
            var result = SlugFormatter.ToSlug(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Red & Blue", "red-and-blue")]
        [InlineData("Green & blue & pink & purple", "green-and-blue-and-pink-and-purple")]
        [InlineData("&", "and")]
        [InlineData("& start", "and-start")]
        [InlineData("End &", "end-and")]
        [InlineData("S&&P", "s-and-and-p")]
        public void ToSlug_WithAmpserand_ConvertsToAnd(string input, string expected)
        {
            var result = SlugFormatter.ToSlug(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1.2.3", "1-2-3")]
        [InlineData("1,2,3", "1-2-3")]
        [InlineData("en–dash", "en-dash")]
        [InlineData("em—dash", "em-dash")]
        [InlineData("en––double––dash", "en-double-dash")]
        [InlineData(".dots....ahoy!", "dots-ahoy")]
        [InlineData("underscore_between_words", "underscore-between-words")]
        [InlineData("1:30", "1-30")]
        [InlineData("2+2=5", "2-2-5")]
        [InlineData("this/that", "this-that")]
        [InlineData("this\\that", "this-that")]
        public void ToSlug_WithSeparatorPunctuation_ConvertsToDash(string input, string expected)
        {
            var result = SlugFormatter.ToSlug(input);

            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(" with a space at start", "with-a-space-at-start")]
        [InlineData("with a space at end ", "with-a-space-at-end")]
        [InlineData("   with space at start", "with-space-at-start")]
        [InlineData("with space at end   ", "with-space-at-end")]
        [InlineData("with puntucation at end+=.–:", "with-puntucation-at-end")]
        [InlineData("+=.–:with puntucation at start", "with-puntucation-at-start")]
        public void ToSlug_TrimsExcessNonCharacters(string input, string expected)
        {
            var result = SlugFormatter.ToSlug(input);

            Assert.Equal(expected, result);
        }

        #endregion

        #region CamelCaseToSlug

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void CamelCaseToSlug_WhenNullOrWhitespace_ReturnsEmptyString(string s)
        {
            var result = SlugFormatter.CamelCaseToSlug(s);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData("ThisAndThat", "this-and-that")]
        [InlineData("ofMiceAndMen", "of-mice-and-men")]
        public void CamelCaseToSlug_CanDoBasicConversion(string input, string expected)
        {
            var result = SlugFormatter.CamelCaseToSlug(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("CostInGBP", "cost-in-gbp")]
        [InlineData("WinningWithTheABCMethod", "winning-with-the-abc-method")]
        public void CamelCaseToSlug_KeepsAcronymsIntact(string input, string expected)
        {
            var result = SlugFormatter.CamelCaseToSlug(input);

            Assert.Equal(expected, result);
        }

        #endregion
    }
}
