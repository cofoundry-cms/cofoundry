namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPagePathHelper"/>.
/// </summary>
public class PagePathHelper : IPagePathHelper
{
    const string PATH_DELIMITER = "/";

    /// <inheritdoc/>
    public string StandardizePath(string? path)
    {
        return StandardizePathWithoutLocale(path, null);
    }

    /// <inheritdoc/>
    public string StandardizePathWithoutLocale(string? path, ActiveLocale? currentLocale)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return PATH_DELIMITER;
        }
        if (!path.StartsWith(PATH_DELIMITER))
        {
            path = PATH_DELIMITER + path;
        }

        // Remove the current locale if it's included in the path
        if (currentLocale != null && path.StartsWith(PATH_DELIMITER + currentLocale.IETFLanguageTag, StringComparison.OrdinalIgnoreCase))
        {
            path = path.Remove(0, currentLocale.IETFLanguageTag.Length + 1);

            // If we accidently removed the starting slash in the above operation, add it again.
            // Example case: path = "en-ca"
            if (!path.StartsWith(PATH_DELIMITER))
            {
                path = PATH_DELIMITER + path;
            }
        }

        if (path == PATH_DELIMITER)
        {
            return path;
        }

        // Remove trailing slash
        return path.TrimEnd(PATH_DELIMITER[0]);
    }
}
