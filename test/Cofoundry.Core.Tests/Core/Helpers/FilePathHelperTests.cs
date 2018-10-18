using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Core.Tests
{
    public class FilePathHelperTests
    {
        #region CleanFileName

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void CleanFileName_WhenNullOrWhitespace_ReturnsEmptyString(string s)
        {
            var result = FilePathHelper.CleanFileName(s);

            Assert.Equal(result, string.Empty);
        }

        [Theory]
        [InlineData("Test Filename")]
        [InlineData("this-that, & other's")]
        [InlineData("New stuff final version 21 Apr 2014")]
        [InlineData("Crème brûlée")]
        [InlineData("Что-то хорошее")]
        public void CleanFileName_WhenValid_ReturnsSame(string s)
        {
            var result = FilePathHelper.CleanFileName(s);

            Assert.Equal(s, result);
        }

        [Theory]
        [InlineData("Test Filename?", "Test Filename")]
        [InlineData("this-that > other's", "this-that  other's")]
        [InlineData("New stuff “final version” 21/04/2014", "New stuff “final version” 21042014")]
        [InlineData("Crème brûlée \\ 40", "Crème brûlée  40")]
        [InlineData("*Что-то |хорошее|", "Что-то хорошее")]
        [InlineData("\0*<>/\\:?|\"", "")]
        [InlineData("\u0009\u000A\u000D\u0020", "")]
        [InlineData("CON", "")]
        [InlineData("CON1", "CON1")]
        [InlineData("PRN", "")]
        [InlineData("PRN2", "PRN2")]
        [InlineData("AUX", "")]
        [InlineData("NUL", "")]
        [InlineData("COM1", "")]
        [InlineData("COM2", "")]
        [InlineData("COM3", "")]
        [InlineData("COM4", "")]
        [InlineData("COM5", "")]
        [InlineData("COM6", "")]
        [InlineData("COM7", "")]
        [InlineData("COM8", "")]
        [InlineData("COM9", "")]
        [InlineData("LPT1", "")]
        [InlineData("LPT2", "")]
        [InlineData("LPT3", "")]
        [InlineData("LPT4", "")]
        [InlineData("LPT5", "")]
        [InlineData("LPT6", "")]
        [InlineData("LPT7", "")]
        [InlineData("LPT8", "")]
        [InlineData("LPT9", "")]
        public void CleanFileName_WhenInvalid_RemovesCharacters(string input, string expected)
        {
            var result = FilePathHelper.CleanFileName(input);

            // TODO: removing control chars: string output = new string(input.Where(c => !char.IsControl(c)).ToArray());

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("$IDLE$", "IDLE")]
        [InlineData("$BOOT", "BOOT")]
        [InlineData(" test ", "test")]
        [InlineData(" $BOOT$ ", "BOOT")]
        public void CleanFileName_LeadingOrTrailingInvalidChars_IsRemoved(string input, string expected)
        {
            var result = FilePathHelper.CleanFileName(input, expected);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("\0*<>/\\:?|\"", "Example")]
        [InlineData("AUX", "Test")]
        public void CleanFileName_NoValidCharacters_ReturnsDefault(string input, string expected)
        {
            var result = FilePathHelper.CleanFileName(input, expected);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Test Filename", "Example")]
        [InlineData("this-that, & other's", "Test")]
        public void CleanFileName_Valid_DoesNotReturnDefault(string input, string defaultValue)
        {
            var result = FilePathHelper.CleanFileName(input, defaultValue);

            Assert.Equal(input, result);
        }

        #endregion

        #region FileExtensionContainsInvalidChars

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void FileExtensionContainsInvalidChars_WhenNullOrWhitespace_ReturnsFalse(string s)
        {
            var result = FilePathHelper.FileExtensionContainsInvalidChars(s);

            Assert.False(result);
        }

        [Theory]
        [InlineData(".gif")]
        [InlineData("gif")]
        [InlineData(".this-that, & other's")]
        [InlineData(".WOMBLE")]
        [InlineData("COM1")]
        [InlineData("PRN")]
        [InlineData("$LINE")]
        [InlineData("Crème brûlée")]
        [InlineData(".Что-то хорошее")]
        public void FileExtensionContainsInvalidChars_WhenValid_ReturnsFalse(string s)
        {
            var result = FilePathHelper.FileExtensionContainsInvalidChars(s);

            Assert.False(result);
        }

        [Theory]
        [InlineData(".gif.gif")]
        [InlineData("gif.gif")]
        [InlineData(".bob?b")]
        [InlineData(".bob\0b")]
        [InlineData(".bob*b")]
        [InlineData(".bob<b")]
        [InlineData(".bob>b")]
        [InlineData(".bob/b")]
        [InlineData(".bob\\b")]
        [InlineData(".bob:b")]
        [InlineData(".bob|b")]
        [InlineData(".bob\"b")]
        public void FileExtensionContainsInvalidChars_WhenInvalid_ReturnsTrue(string s)
        {
            var result = FilePathHelper.FileExtensionContainsInvalidChars(s);

            Assert.True(result);
        }

        #endregion

        #region CombineVirtualPath

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void CombineVirtualPath_WhenNullOrWhitespace_ReturnsEmptyPath(string s)
        {
            var result = FilePathHelper.CombineVirtualPath(s);

            Assert.Equal("/", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void CombineVirtualPath_WhenMultipleNullOrWhitespace_ReturnsPath(string s)
        {
            var result = FilePathHelper.CombineVirtualPath(s, s, s);

            Assert.Equal("/", result);
        }

        [Theory]
        [InlineData("path", "to", "somefile.exe", "/path/to/somefile.exe")]
        [InlineData("/path/", "/to/", "/somefile.exe", "/path/to/somefile.exe")]
        [InlineData("//path", "TO ", "some-file.exe", "/path/TO /some-file.exe")]
        [InlineData("\\path\\", "\\to\\", "/somefile.exe", "/path/to/somefile.exe")]
        public void CombineVirtualPath_WhenValid_ReturnsCombined(string path1, string path2, string path3, string expected)
        {
            var result = FilePathHelper.CombineVirtualPath(path1, path2, path3);

            Assert.Equal(expected, result);
        }

        #endregion
    }
}
