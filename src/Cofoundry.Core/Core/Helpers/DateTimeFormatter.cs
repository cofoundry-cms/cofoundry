using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    public static class DateTimeFormatter
    {
        private static readonly Lazy<SortedList<double, Func<TimeSpan, string>>> relativeDateOffsets = new Lazy<SortedList<double, Func<TimeSpan, string>>>(() =>
        {
            return new SortedList<double, Func<TimeSpan, string>>
            {
                { 0.75, _ => "less than a minute"},
                { 1.5, _ => "about a minute"},
                { 45, x => $"{x.TotalMinutes:F0} minutes"},
                { 90, x => "about an hour"},
                { 1440, x => $"about {x.TotalHours:F0} hours"},
                { 2880, x => "a day"},
                { 43200, x => $"{x.TotalDays:F0} days"},
                { 86400, x => "about a month"},
                { 525600, x => $"{x.TotalDays / 30:F0} months"},
                { 1051200, x => "about a year"},
                { double.MaxValue, x => $"{x.TotalDays / 365:F0} years"}
            };
        });

        /// <summary>
        /// Converts a datetime into a relative string description compared to the time 
        /// right now, e.g. 2 days ago or about a minute from now.
        /// </summary>
        /// <param name="dateTime">The date time to convert.</param>
        public static string ToRelativeDateString(DateTime dateTime)
        {
            var timeDifference = DateTime.Now - dateTime;
            string suffix = timeDifference.TotalMinutes > 0 ? " ago" : " from now";
            timeDifference = new TimeSpan(Math.Abs(timeDifference.Ticks));

            return relativeDateOffsets
                .Value
                .First(n => timeDifference.TotalMinutes < n.Key)
                .Value(timeDifference) + suffix;
        }
    }
}
