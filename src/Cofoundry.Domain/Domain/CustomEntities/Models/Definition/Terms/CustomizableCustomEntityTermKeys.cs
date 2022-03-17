namespace Cofoundry.Domain;

/// <summary>
/// Constants for customizable custom entity term keys.
/// </summary>
public static class CustomizableCustomEntityTermKeys
{
    public static readonly string Title = "Title";
    public static readonly string UrlSlug = "UrlSlug";

    /// <summary>
    /// The default custom entity terms to apply if none other are defined.
    /// </summary>
    public static readonly Dictionary<string, string> Defaults = new Dictionary<string, string>()
    {
        { Title, Title },
        { UrlSlug, "Url Slug" }
    };
}
