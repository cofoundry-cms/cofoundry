namespace Cofoundry.Domain;

public class GetActiveLocaleByIdQuery : IQuery<ActiveLocale>
{
    public GetActiveLocaleByIdQuery()
    {
    }

    public GetActiveLocaleByIdQuery(int localeId)
    {
        LocaleId = localeId;
    }

    public int LocaleId { get; set; }
}
