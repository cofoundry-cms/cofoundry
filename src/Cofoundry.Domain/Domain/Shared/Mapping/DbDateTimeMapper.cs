using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to specify a datetime as DateTimeKind.UTC.
    /// </summary>
    /// <remarks>
    /// This was intended to be replaced with EF type mapping which
    /// is supported in EF Core 2.1, but there are some limitations with
    /// query translations so it didn't seem worth the effort of making the change
    /// until a clear need for it arises.
    /// </remarks>
    public static class DbDateTimeMapper
    {
        public static DateTime AsUtc(DateTime source)
        {
            return DateTime.SpecifyKind(source, DateTimeKind.Utc);
        }

        public static DateTime? AsUtc(DateTime? source)
        {
            if (!source.HasValue) return null;

            return AsUtc(source.Value);
        }
    }
}
