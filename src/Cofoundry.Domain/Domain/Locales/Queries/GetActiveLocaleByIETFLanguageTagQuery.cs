namespace Cofoundry.Domain;

public class GetActiveLocaleByIETFLanguageTagQuery : IQuery<ActiveLocale>
{
    public GetActiveLocaleByIETFLanguageTagQuery()
    {
    }

    public GetActiveLocaleByIETFLanguageTagQuery(string tag)
    {
        IETFLanguageTag = tag;
    }

    [Required]
    public string IETFLanguageTag { get; set; }
}