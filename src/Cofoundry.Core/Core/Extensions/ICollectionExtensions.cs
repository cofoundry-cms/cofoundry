using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Removes from the target collection all elements that match the specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of elements in the target collection.</typeparam>
        /// <param name="collection">The target collection.</param>
        /// <param name="predicate">Optional predicate used to match elements.</param>
        /// <exception cref="ArgumentNullException">
        /// The target collection is a null reference.
        /// <br />-or-<br />
        /// The match predicate is a null reference.
        /// </exception>
        /// <returns>Returns the number of elements removed.</returns>
        public static int RemoveAll<T>(this ICollection<T> collection, Predicate<T> predicate = null)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            int count = 0;

            for (int i = collection.Count - 1; i >= 0; i--)
            {
                var el = collection.ElementAt(i);
                if (predicate == null || predicate(el))
                {
                    collection.Remove(el);
                    count++;
                }
            }

            return count;
        }
    }
}
