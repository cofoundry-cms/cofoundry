using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Time
{
    /// <summary>
    /// Utilities for converting between different timezones.
    /// </summary>
    public interface ITimeZoneConverter
    {
        /// <summary>
        /// Converts a datetime from UTC to the specified timezone.
        /// </summary>
        /// <param name="dateTime">UTC timezone to convert from.</param>
        /// <param name="timeZoneId">Timezone to convert to.</param>
        DateTimeOffset FromUtcToOffset(DateTime dateTime, string timeZoneId);
    }
}
