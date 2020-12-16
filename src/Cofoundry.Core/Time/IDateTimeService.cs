using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Time
{
    /// <summary>
    /// Injectable DateTime source to allow for easier testing 
    /// of date dependent functions.
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// Gets a System.DateTimeOffset object whose date and time are set to the current
        //  Coordinated Universal Time (UTC) date and time and whose offset is System.TimeSpan.Zero.
        /// </summary>
        DateTimeOffset OffsetUtcNow();

        /// <summary>
        /// Gets a System.DateTime object that is set to the current date and time on this
        //  computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        DateTime UtcNow();
    }
}
