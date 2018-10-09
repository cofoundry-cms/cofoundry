using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used for converting the asset file update date to a filestamp
    /// string that can be used in urls as a cache breaker.
    /// </summary>
    public static class AssetFileStampHelper
    {
        private static DateTime COFOUNDRY_EPOCH = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const long TICKS_MULTIPLIER = 1000;

        /// <summary>
        /// <para>
        /// Converts the specified DateTime into a filestamp. The datetime
        /// is expected to be UTC with a precision exquivalent to the datetime2(4)
        /// MS Sql type.
        /// </para>
        /// <para>
        /// To reduce the size of the filestamp it is also assumed that no assets
        /// will have been created prior to Cofoundry existing - the date 2010-1-1
        /// is used as the epoch.
        /// </para>
        /// </summary>
        /// <param name="fileUpdateDate">The UTC DateTime to convert.</param>
        public static string ToFileStamp(DateTime fileUpdateDate)
        {
            if (fileUpdateDate < COFOUNDRY_EPOCH) throw new ArgumentException("Asset file update date is prior to the Cofoundry epoch: " + fileUpdateDate, nameof(fileUpdateDate));

            var ticks = fileUpdateDate.Ticks - COFOUNDRY_EPOCH.Ticks;
            var nanoSeconds = ticks / TICKS_MULTIPLIER;

            return nanoSeconds.ToString();
        }

        /// <summary>
        /// Converts a filestamp back to a UTC Date. If the date is unparsable
        /// then null is returned.
        /// </summary>
        /// <param name="fileStamp">Filestamp created with ToFileStamp, to be converted back to a UTC datetime.</param>
        public static DateTime? ToDate(long fileStamp)
        {
            if (fileStamp < 0 || fileStamp > long.MaxValue / TICKS_MULTIPLIER) return null;

            var stampTicks = fileStamp * TICKS_MULTIPLIER;
            var ticks = stampTicks + COFOUNDRY_EPOCH.Ticks;
            var result = new DateTime(ticks, DateTimeKind.Utc);

            return result;
        }
    }
}
