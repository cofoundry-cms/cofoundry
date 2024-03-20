namespace Cofoundry.Core.Time.Internal;

public class DateTimeService : IDateTimeService
{
    private readonly TimeProvider _timeProvider;

    public DateTimeService(
        TimeProvider timeProvider
        )
    {
        _timeProvider = timeProvider;
    }

    public virtual DateTime UtcNow()
    {
        return OffsetUtcNow().DateTime;
    }

    public virtual DateTimeOffset OffsetUtcNow()
    {
        return _timeProvider.GetUtcNow();
    }
}
