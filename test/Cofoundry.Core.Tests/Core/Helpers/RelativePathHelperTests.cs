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

        #region IsWellFormattedAndEqual

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void IsWellFormattedAndEqual_WhenNullOrEmpty_ReturnsFalse(string path)
        {
            var bothResult = RelativePathHelper.IsWellFormattedAndEqual(path, path);
            var path1Result = RelativePathHelper.IsWellFormattedAndEqual(path, "/mypath");
            var path2Result = RelativePathHelper.IsWellFormattedAndEqual("/mypath", path);

            Assert.False(bothResult);
            Assert.False(path1Result);
            Assert.False(path2Result);
        }

        [Theory]
        [InlineData("https://www.cofoundry.org/test")]
        [InlineData("../bad-path")]
        [InlineData("no-fun")]
        [InlineData("/!invalid>path<chars")]
        public void IsWellFormattedAndEqual_WhenInvalid_ReturnsFalse(string path)
        {
            var bothResult = RelativePathHelper.IsWellFormattedAndEqual(path, path);
            var path1Result = RelativePathHelper.IsWellFormattedAndEqual(path, "/mypath");
            var path2Result = RelativePathHelper.IsWellFormattedAndEqual("/mypath", path);

            Assert.False(bothResult);
            Assert.False(path1Result);
            Assert.False(path2Result);
        }

        [Theory]
        [InlineData("/test", "/test")]
        [InlineData("/test/", "/test")]
        [InlineData("/test/myfile.jpg", "/test/myfile.jpg")]
        [InlineData("/test/deep/deeper/", "/test/deep/deeper")]
        public void IsWellFormattedAndEqual_WhenValid_ReturnsTrue(string path1, string path2)
        {
            var bothResult = RelativePathHelper.IsWellFormattedAndEqual(path1, path1);
            var path1Result = RelativePathHelper.IsWellFormattedAndEqual(path1, path2);
            var path2Result = RelativePathHelper.IsWellFormattedAndEqual(path2, path1);

            Assert.True(bothResult);
            Assert.True(path1Result);
            Assert.True(path2Result);
        }

        [Theory]
        [InlineData("~/test", "/test")]
        [InlineData("~/test/deep", "/test/deep")]
        [InlineData("~/test?var=test", "/test")]
        [InlineData("~/test#test", "/test")]
        public void IsWellFormattedAndEqual_ValidWithAppReleative_ReturnsTrue(string path1, string path2)
        {
            var bothResult = RelativePathHelper.IsWellFormattedAndEqual(path1, path1);
            var path1Result = RelativePathHelper.IsWellFormattedAndEqual(path1, path2);
            var path2Result = RelativePathHelper.IsWellFormattedAndEqual(path2, path1);

            Assert.True(bothResult);
            Assert.True(path1Result);
            Assert.True(path2Result);
        }

        [Theory]
        [InlineData("/test?test=true", "/test")]
        [InlineData("/test/deep?a=b&y=x", "/test/deep?a=c&y=f")]
        public void IsWellFormattedAndEqual_ValidWithQueryString_ReturnsTrue(string path1, string path2)
        {
            var bothResult = RelativePathHelper.IsWellFormattedAndEqual(path1, path1);
            var path1Result = RelativePathHelper.IsWellFormattedAndEqual(path1, path2);
            var path2Result = RelativePathHelper.IsWellFormattedAndEqual(path2, path1);

            Assert.True(bothResult);
            Assert.True(path1Result);
            Assert.True(path2Result);
        }

        [Theory]
        [InlineData("/test#test", "/test")]
        [InlineData("/test/#test", "/test")]
        [InlineData("/test.html#test", "/test.html")]
        [InlineData("/test/deep/#test", "/test/deep")]
        public void IsWellFormattedAndEqual_ValidWithFragment_ReturnsTrue(string path1, string path2)
        {
            var bothResult = RelativePathHelper.IsWellFormattedAndEqual(path1, path1);
            var path1Result = RelativePathHelper.IsWellFormattedAndEqual(path1, path2);
            var path2Result = RelativePathHelper.IsWellFormattedAndEqual(path2, path1);

            Assert.True(bothResult);
            Assert.True(path1Result);
            Assert.True(path2Result);
        }

        #endregion
    }
}
