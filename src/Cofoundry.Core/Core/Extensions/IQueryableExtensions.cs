namespace Cofoundry.Core;

/// <summary>
/// Extension methods for <see cref="IQueryable{T}"/> types.
/// </summary>
public static class IQueryableExtensions
{
    extension<T>(IQueryable<T?> source) where T : struct
    {
        /// <summary>
        /// Removes nullable entries from the sequence.
        /// </summary>
        public IQueryable<T> WhereNotNull()
        {
            return source
                .Where(s => s != null)
                .Cast<T>();
        }
    }

    extension<T>(IQueryable<T?> source)
    {
        /// <summary>
        /// Removes nullable entries from the sequence.
        /// </summary>
        public IQueryable<T> WhereNotNull()
        {
            return source
                .Where(s => s != null)
                .Cast<T>();
        }
    }

    extension(IQueryable<string?> source)
    {
        /// <summary>
        /// Removes nullable null or empty strings from the sequence.
        /// </summary>
        public IQueryable<string> WhereNotNullOrEmpty()
        {
            return source
                .Where(s => !string.IsNullOrEmpty(s))
                .Cast<string>();
        }

        /// <summary>
        /// Removes nullable null, empty or strings that contain only whitespace 
        /// from the sequence.
        /// </summary>
        public IQueryable<string> WhereNotNullOrWhitespace()
        {
            return source
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Cast<string>();
        }
    }
}
