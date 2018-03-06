using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to specify a datetime as DateTimeKind.UTC. To be used
    /// until EF core supports specfying the DateTimeKind (supported in 2.1)
    /// </summary>
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
