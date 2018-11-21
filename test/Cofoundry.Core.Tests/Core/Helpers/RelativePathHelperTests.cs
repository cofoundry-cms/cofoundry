using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Core.Tests
{
    public class RelativePathHelperTests
    {
        #region CombineVirtualPath

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void Combine_WhenNullOrWhitespace_ReturnsEmptyPath(string s)
        {
            var result = RelativePathHelper.Combine(s);

            Assert.Equal("/", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void Combine_WhenMultipleNullOrWhitespace_ReturnsPath(string s)
        {
            var result = RelativePathHelper.Combine(s, s, s);

            Assert.Equal("/", result);
        }

        [Theory]
        [InlineData("path", "to", "somefile.exe", "/path/to/somefile.exe")]
        [InlineData("/path/", "/to/", "/somefile.exe", "/path/to/somefile.exe")]
        [InlineData("//path", "TO ", "some-file.exe", "/path/TO /some-file.exe")]
        [InlineData("\\path\\", "\\to\\", "/somefile.exe", "/path/to/somefile.exe")]
        public void Combine_WhenValid_ReturnsCombined(string path1, string path2, string path3, string expected)
        {
            var result = RelativePathHelper.Combine(path1, path2, path3);

            Assert.Equal(expected, result);
        }

        #endregion
    }
}
