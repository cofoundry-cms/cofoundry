using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Core.Tests
{
    public class SqlCharValidatorTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("      ")]
        [InlineData("               ")]
        public void IsValid_WhenNullOrWhitespace_ReturnsFalse(string s)
        {
            var result = SqlCharValidator.IsValid(s, 6);

            Assert.False(result);
        }

        [Theory]
        [InlineData("TST")]
        [InlineData("OB-NO-B")]
        [InlineData("123456789")]
        [InlineData("testing")]
        [InlineData("Crème brûlée")]
        public void IsValid_WhenValid_ReturnsTrue(string s)
        {
            var result = SqlCharValidator.IsValid(s, s.Length);

            Assert.True(result);
        }

        [Theory]
        [InlineData("diakrī́nō")]
        [InlineData("Что-то хорошее")]
        public void IsValid_WithUnicode_ReturnsFalse(string s)
        {
            var result = SqlCharValidator.IsValid(s, s.Length);

            Assert.False(result);
        }

        [Theory]
        [InlineData("TST", 2)]
        [InlineData("OB-NO-B", 8)]
        [InlineData("123456789", 20)]
        [InlineData("TESTING", -20)]
        [InlineData("zero", 0)]
        public void IsValid_WhenInvalidLength_ReturnsFalse(string s, int length)
        {
            var result = SqlCharValidator.IsValid(s, length);

            Assert.False(result);
        }
    }
}
