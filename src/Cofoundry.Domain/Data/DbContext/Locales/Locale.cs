namespace Cofoundry.Domain.Data;

public class Locale
{
    public int LocaleId { get; set; }

    public int? ParentLocaleId { get; set; }

    public Locale? ParentLocale { get; set; }

    public string IETFLanguageTag { get; set; } = string.Empty;

    public string LocaleName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public ICollection<Locale> ChildLocales { get; set; } = new List<Locale>();
}
