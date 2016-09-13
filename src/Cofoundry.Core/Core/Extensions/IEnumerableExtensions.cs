using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Core
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Removes nullable entries from the collection
        /// </summary>
        public static IEnumerable<T> FilterNotNull<T>(this IEnumerable<Nullable<T>> source)
            where T : struct
        {
            return source
                .Where(s => s.HasValue)
                .Select(s => s.Value);
        }
    }
}
