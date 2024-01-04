namespace Cofoundry.Domain.Data;

public class PageDirectoryLocale : ICreateAuditable
{
    public int PageDirectoryLocaleId { get; set; }

    public int PageDirectoryId { get; set; }

    private PageDirectory? _pageDirectory;
    public PageDirectory PageDirectory
    {
        get => _pageDirectory ?? throw NavigationPropertyNotInitializedException.Create<PageDirectoryLocale>(nameof(Creator));
        set => _pageDirectory = value;
    }

    public int LocaleId { get; set; }

    private Locale? _locale;
    public Locale Locale
    {
        get => _locale ?? throw NavigationPropertyNotInitializedException.Create<PageDirectoryLocale>(nameof(Locale));
        set => _locale = value;
    }

    public string UrlPath { get; set; } = string.Empty;

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<PageDirectoryLocale>(nameof(Creator));
        set => _creator = value;
    }
}
