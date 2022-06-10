using Cofoundry.Core.Mail;
using Cofoundry.Core.Mail.Internal;

namespace Cofoundry.Domain.Tests.Integration.Mocks;

public class AuditableMailDispatchSession : IMailDispatchSession
{
    private readonly DebugMailDispatchSession _wrappedInstance;
    private readonly List<MailMessage> _messages = new List<MailMessage>();

    public AuditableMailDispatchSession(
        MailSettings mailSettings,
        IPathResolver pathResolver
        )
    {
        _wrappedInstance = new DebugMailDispatchSession(mailSettings, pathResolver);
    }

    /// <summary>
    /// Returns the number of mail messages that have been registered to this session
    /// that matches the <paramref name="predicate"/>. Use this to check to check
    /// that a set of mail messages have been dispatched.
    /// </summary>
    /// <param name="predicate">An expression to filter mail messages by.</param>
    /// <returns>The number of mail messages matched by the <paramref name="predicate"/>.</returns>
    public int CountDispatched(Func<MailMessage, bool> predicate)
    {
        return _messages
            .Where(predicate)
            .Count();
    }

    /// <summary>
    /// Return the number of mail messages that have been registered to this session that match the
    /// specified search parameters.
    /// </summary>
    /// <param name="toEmail">The email address the message was dispatched to (case-insensitive comparisson.</param>
    /// <param name="textPartsToMatch">
    /// A set of text parts to look for in the message body. All text parts must be found in either
    /// the text or html body to be matched. Comparisson is case-insensitive.
    /// </param>
    public int CountDispatched(string toEmail, params string[] textPartsToMatch)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(toEmail);

        return CountDispatched(m =>
            m.To.Address.Equals(toEmail, StringComparison.OrdinalIgnoreCase)
            && MatchesBody(m, textPartsToMatch)
            );
    }

    private bool MatchesBody(MailMessage message, string[] textToMatch)
    {
        var matches = EnumerableHelper.Enumerate(textToMatch);

        if (!string.IsNullOrWhiteSpace(message.TextBody)
            && matches.All(m => message.TextBody.Contains(m, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(message.HtmlBody)
            && matches.All(m => message.HtmlBody.Contains(m, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }

    public void Add(MailMessage mailMessage)
    {
        _messages.Add(mailMessage);
        _wrappedInstance.Add(mailMessage);
    }

    public void Dispose()
    {
        _wrappedInstance.Dispose();
    }

    public Task FlushAsync()
    {
        return _wrappedInstance.FlushAsync();
    }
}
