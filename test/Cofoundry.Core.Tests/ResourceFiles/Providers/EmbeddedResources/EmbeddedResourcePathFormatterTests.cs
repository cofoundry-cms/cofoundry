using Cofoundry.Core.ResourceFiles;
using Xunit;

namespace Cofoundry.Core.Tests.ResourceFiles
{
    public class EmbeddedResourcePathFormatterTests
    {
        [Theory]
        [InlineData("dir-1/dir-2", "dir_1.dir_2")]
        [InlineData("one/two.three,;four@#five%six^-123", "one.two_three__four__five_six__123")]
        [InlineData("test[]{}()/ABC_/!:<>", "test______.ABC_.____")]
        public void ConvertFromVirtualDirectory_WhenInvalidChars_Corrects(string input, string expected)
        {
            var result = EmbeddedResourcePathFormatter.ConvertFromVirtualDirectory(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("~/directory/", "directory")]
        [InlineData("/Parent/Sub/Sub/", "Parent.Sub.Sub")]
        public void ConvertFromVirtualDirectory_WhenSuperfluousDelimiters_Removes(string input, string expected)
        {
            var result = EmbeddedResourcePathFormatter.ConvertFromVirtualDirectory(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("dir-1.dir-2", "dir_1.dir_2")]
        [InlineData("one/two.three,;four@#five%six^-123", "one_two.three__four__five_six__123")]
        [InlineData("test[]{}()/ABC_.!:<>", "test_______ABC_.____")]
        public void CleanPath_WhenInvalidChars_Corrects(string input, string expected)
        {
            var result = EmbeddedResourcePathFormatter.CleanPath(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(".directory.", "directory")]
        [InlineData(".Parent.Sub.Sub.", "Parent.Sub.Sub")]
        public void CleanPath_WhenSuperfluousDelimiters_Removes(string input, string expected)
        {
            var result = EmbeddedResourcePathFormatter.CleanPath(input);

            Assert.Equal(expected, result);
        }
    }
}
