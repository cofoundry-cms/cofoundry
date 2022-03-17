namespace Cofoundry.Core.Mail.Internal;

/// <summary>
/// This is a simple service implementation that defers
/// logic to the <see cref="IMailDispatchSession"/> implementation
/// returned from <see cref="IMailDispatchSessionFactory"/>.
/// </summary>
/// <inheritdoc/>
public class DefaultMailDispatchService : IMailDispatchService
{
    private readonly IMailDispatchSessionFactory _mailDispatchSessionFactory;

    public DefaultMailDispatchService(
        IMailDispatchSessionFactory mailDispatchSessionFactory
        )
    {
        _mailDispatchSessionFactory = mailDispatchSessionFactory;
    }

    public async Task DispatchAsync(MailMessage message)
    {
        using var session = CreateSession();

        session.Add(message);
        await session.FlushAsync();
    }

    public IMailDispatchSession CreateSession()
    {
        return _mailDispatchSessionFactory.Create();
    }
}
