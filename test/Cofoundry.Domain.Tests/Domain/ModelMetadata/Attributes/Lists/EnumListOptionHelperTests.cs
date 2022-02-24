using Cofoundry.Domain.Internal;
using FluentAssertions;
using FluentAssertions.Execution;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace Cofoundry.Domain.Tests.ModelMetadata.Attributes.Lists
{
    public class EnumListOptionHelperTests
    {
        [Fact]
        public void CanConvert()
        {
            var result = EnumListOptionHelper.GetOptions(typeof(TestEnum))?.ToList();

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(5);

                result[0].Text.Should().Be("None");
                result[0].Value.Should().Be("None");

                result[1].Text.Should().Be("Item one");
                result[1].Value.Should().Be("ItemOne");

                result[2].Text.Should().Be("Item 2");
                result[2].Value.Should().Be("ItemTwo");

                result[3].Text.Should().Be("Item three and four");
                result[3].Value.Should().Be("ItemThreeAndFour");

                result[4].Text.Should().Be("88");
                result[4].Value.Should().Be("ItemEightyEight");
            }
        }

        public enum TestEnum
        {
            None = 0,
            ItemOne = 1,
            [Description("Item 2")]
            ItemTwo = 2,
            ItemThreeAndFour,
            [Description("88")]
            ItemEightyEight
        }
    }
}
