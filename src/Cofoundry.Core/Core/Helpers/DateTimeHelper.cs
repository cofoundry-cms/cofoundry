using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Removes the millisecond portion of a DateTime.
        /// </summary>
        /// <param name="dateTime">DateTime to truncate.</param>
        /// <returns>A new DateTime instance.</returns>
        public static DateTime TruncateMilliseconds(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
        }
    }
}
