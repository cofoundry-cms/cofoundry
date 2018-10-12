using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class AssetFileStampHelperTests
    {
        #region CreateFileStamp

        [Theory]
        [InlineData("2009/12/31 23:59:59.9999")]
        [InlineData("1872/04/01 00:00:59.0012")]
        public void CreateFileStamp_WhenPriorToEpoch_Throws(string fileUpdateDate)
        {
            var dateToTest = DateTime.Parse(fileUpdateDate);

            Assert.Throws<ArgumentException>(() => AssetFileStampHelper.ToFileStamp(dateToTest));
        }

        [Theory]
        [InlineData("2010/01/01 00:00:00", "0")]
        [InlineData("2010/01/01 00:00:59.0012", "590012")]
        [InlineData("2018/08/16 13:22:43.1234", "2721217631234")]
        public void CreateFileStamp_WithValidData_CreatesCorrectFileStamp(string fileUpdateDate, string expected)
        {
            var dateToTest = DateTime.Parse(fileUpdateDate);

            var result = AssetFileStampHelper.ToFileStamp(dateToTest);

            Assert.Equal(expected, result);
        }

        #endregion

        #region ToDate

        [Theory]
        [InlineData(-2)]
        [InlineData(-590012)]
        [InlineData(9223372036854775807)]
        public void ToDate_WithInvalidData_ReturnsNull(long fileStamp)
        {
            var result = AssetFileStampHelper.ToDate(fileStamp);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(0, "2010/01/01 00:00:00")]
        [InlineData(590012, "2010/01/01 00:00:59.0012")]
        [InlineData(2721217631234, "2018/08/16 13:22:43.1234")]
        public void ToDate_WithValidData_ReturnsCorrectResult(long fileStamp, string expected)
        {
            var expectedDate = DateTime.Parse(expected);

            var result = AssetFileStampHelper.ToDate(fileStamp);

            Assert.Equal(expectedDate, result);
            Assert.Equal(DateTimeKind.Utc, result.Value.Kind);
        }

        #endregion
    }

}
