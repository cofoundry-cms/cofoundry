﻿namespace Cofoundry.Core.Time.Internal;

/// <summary>
/// Utilities for converting between different timezones.
/// </summary>
public class TimeZoneConverter : ITimeZoneConverter
{
    /// <summary>
    /// Converts a datetime from UTC to the specified timezone.
    /// </summary>
    /// <param name="dateTime">UTC timezone to convert from.</param>
    /// <param name="timeZoneId">Timezone to convert to.</param>
    public virtual DateTimeOffset FromUtcToOffset(DateTime dateTime, string timeZoneId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZoneId);

        if (dateTime.Kind == DateTimeKind.Local)
        {
            throw new InvalidOperationException("Cannot convert from a local timezone.");
        }

        // will throw a TimeZoneNotFoundException if not found.
        var timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var localDate = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, timezone);

        return localDate;
    }
}
