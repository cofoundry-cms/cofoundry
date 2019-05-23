using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Core.Tests
{
    public class TypeHelperTests
    {
        [Fact]
        public void GetCollectionTypeOrNull_WhenNull_ReturnsNull()
        {
            var result = TypeHelper.GetCollectionTypeOrNull(null);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(Dictionary<string, int>))]
        [InlineData(typeof(double))]
        [InlineData(typeof(TheoryAttribute))]
        public void GetCollectionTypeOrNull_WhenNonCollectionType_ReturnsNull(Type type)
        {
            var result = TypeHelper.GetCollectionTypeOrNull(type);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(typeof(string[]), typeof(string))]
        [InlineData(typeof(TheoryAttribute[]), typeof(TheoryAttribute))]
        [InlineData(typeof(IEnumerable<int>), typeof(int))]
        [InlineData(typeof(ICollection<TheoryAttribute>), typeof(TheoryAttribute))]
        [InlineData(typeof(List<double>), typeof(double))]
        public void GetCollectionTypeOrNull_WhenCollectionType_ReturnsInnerType(Type typeToCheck, Type expected)
        {
            var result = TypeHelper.GetCollectionTypeOrNull(typeToCheck);

            Assert.Equal(expected, result);
        }
    }
}
