using FluentAssertions;
using FluentAssertions.Execution;
using System;
using Xunit;

namespace Cofoundry.Core.Core.Parsers
{
    public class EnumParserTests
    {
        public enum TestEnum
        {
            Default = 0,
            Value1 = 1,
            Value5 = 5,
            Value10 = 10
        }

        [Fact]
        public void ParseOrNull_WhenValidString_Parses()
        {
            var result = EnumParser.ParseOrNull<TestEnum>("Value5");

            result.Should().Be(TestEnum.Value5);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Value")]
        [InlineData(null)]
        public void ParseOrNull_WhenInvalidString_ReturnsNull(string value)
        {
            var result = EnumParser.ParseOrNull<TestEnum>(value);

            result.Should().BeNull();
        }

        [Fact]
        public void ParseOrNull_WhenValidInt_Parses()
        {
            var result = EnumParser.ParseOrNull<TestEnum>(5);

            result.Should().Be(TestEnum.Value5);
        }

        [Fact]
        public void ParseOrNull_WhenInvalidInt_ReturnsNull()
        {
            var result = EnumParser.ParseOrNull<TestEnum>(13);

            result.Should().BeNull();
        }

        [Fact]
        public void ParseOrDefault_WhenInvalidWithNoDefault_ReturnsDefault()
        {
            var result = EnumParser.ParseOrDefault<TestEnum>("Inconceivable");

            result.Should().Be(TestEnum.Default);
        }

        [Theory]
        [InlineData("", TestEnum.Value5)]
        [InlineData("Value", TestEnum.Value1)]
        [InlineData(null, TestEnum.Value5)]
        public void ParseOrDefault_WhenInvalidWithDefault_ReturnsSpecifiedDefault(string value, TestEnum? defaultResult)
        {
            var result = EnumParser.ParseOrDefault<TestEnum>(value, defaultResult);

            result.Should().Be(defaultResult);
        }

        [Fact]
        public void ParseOrThrow_WhenValid_Parses()
        {
            var result = EnumParser.ParseOrThrow<TestEnum>(5);

            result.Should().Be(TestEnum.Value5);
        }

        [Fact]
        public void ParseOrThrow_WhenInvalid_Throws()
        {
            Action sut = () => EnumParser.ParseOrThrow<TestEnum>(4);

            sut
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("4 * valid TestEnum *");
        }
    }
}
