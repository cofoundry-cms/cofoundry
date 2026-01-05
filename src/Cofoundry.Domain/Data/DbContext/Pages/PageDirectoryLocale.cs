namespace Cofoundry.Domain.Data;

public class PageDirectoryLocale : ICreateAuditable
{
    public int PageDirectoryLocaleId { get; set; }

    public int PageDirectoryId { get; set; }

    public PageDirectory PageDirectory
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageDirectoryLocale>(nameof(Creator));
        set;
    }

    public int LocaleId { get; set; }

    public Locale Locale
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageDirectoryLocale>(nameof(Locale));
        set;
    }

    public string UrlPath { get; set; } = string.Empty;

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageDirectoryLocale>(nameof(Creator));
        set;
    }
}
