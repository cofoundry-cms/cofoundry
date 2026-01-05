namespace Cofoundry.Core;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class IEnumerableExtensions
{
    extension<T>(IEnumerable<T?> source) where T : struct
    {
        /// <summary>
        /// Removes nullable entries from the sequence.
        /// </summary>
        public IEnumerable<T> WhereNotNull()
        {
            return source
                .Where(s => s != null)
                .Cast<T>();
        }
    }

    extension<T>(IEnumerable<T?> source)
    {
        /// <summary>
        /// Removes nullable entries from the sequence.
        /// </summary>
        public IEnumerable<T> WhereNotNull()
        {
            return source
                .Where(s => s != null)
                .Cast<T>();
        }
    }

    extension(IEnumerable<string?> source)
    {
        /// <summary>
        /// Removes nullable null or empty strings from the sequence.
        /// </summary>
        public IEnumerable<string> WhereNotNullOrEmpty()
        {
            return source
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>();
        }

        /// <summary>
        /// Removes nullable null, empty or strings that contain only whitespace 
        /// from the sequence.
        /// </summary>
        public IEnumerable<string> WhereNotNullOrWhitespace()
        {
            return source
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Cast<string>();
        }
    }
}
