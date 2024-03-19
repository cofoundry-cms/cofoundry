﻿using Cofoundry.Core.Mail;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Time;
using Cofoundry.Core.Time.Mocks;
using Cofoundry.Domain.Tests.Integration.Mocks;

namespace Cofoundry.Domain.Tests.Integration;

/// <summary>
/// Convenience methods to make it easier to work
/// with mocks and service wrappers set up in the 
/// service collection for testing.
/// </summary>
public class MockServicesHelper
{
    private readonly TestApplicationServiceScope _serviceScope;

    public MockServicesHelper(
        TestApplicationServiceScope serviceScope
        )
    {
        _serviceScope = serviceScope;
    }

    /// <summary>
    /// Sets the date and time used by Cofoundry (via 
    /// <see cref="IDateTimeService"/>) to a specific value.
    /// </summary>
    /// <param name="utcNow">The UTC time to set as the current time.</param>
    public void MockIPAddress(string ipAddress)
    {
        if (_serviceScope.GetService<IClientConnectionService>() is not MockClientConnectionService clientConnectionService)
        {
            throw new Exception($"{nameof(IClientConnectionService)} is expected to be an instance of {nameof(MockClientConnectionService)} in testing");
        }
        clientConnectionService.ClientConnectionInfo.IPAddress = ipAddress;
    }

    /// <summary>
    /// Sets the date and time used by Cofoundry (via 
    /// <see cref="IDateTimeService"/>) to a specific value.
    /// </summary>
    /// <param name="utcNow">The UTC time to set as the current time.</param>
    public void MockDateTime(DateTime utcNow)
    {
        MockDateTime(_serviceScope, utcNow);
    }

    /// <summary>
    /// Sets the date and time used by Cofoundry (via 
    /// <see cref="IDateTimeService"/>) to a specific value.
    /// </summary>
    /// <param name="serviceProvider">The service provider to set mock the date on.</param>
    /// <param name="utcNow">The UTC time to set as the current time.</param>
    public static void MockDateTime(IServiceProvider serviceProvider, DateTime utcNow)
    {
        if (serviceProvider.GetService<IDateTimeService>() is not MockDateTimeService dateTimeService)
        {
            throw new Exception($"{nameof(IDateTimeService)} is expected to be an instance of {nameof(MockDateTimeService)} in testing");
        }
        dateTimeService.MockDateTime = utcNow;
    }

    /// <summary>
    /// Returns the number of messages that have been published to this instance
    /// that matches the <paramref name="predicate"/>. Use this to check to check
    /// that the expected message is published only once.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to look for.</typeparam>
    /// <param name="expression">An expression to filter messages by.</param>
    /// <returns>The number of messages matched by the <paramref name="predicate"/>.</returns>
    public int CountMessagesPublished<TMessage>(Func<TMessage, bool>? predicate)
    {
        if (_serviceScope.GetService<IMessageAggregator>() is not AuditableMessageAggregator auditableMessageAggregator)
        {
            throw new Exception($"{nameof(IMessageAggregator)} is expected to be an instance of {nameof(AuditableMessageAggregator)} in testing");
        }

        if (predicate == null)
        {
            return auditableMessageAggregator.CountMessagesPublished<TMessage>();
        }

        return auditableMessageAggregator.CountMessagesPublished(predicate);
    }

    /// <summary>
    /// Returns the number of messages that have been published to this instance
    /// that matches the <paramref name="predicate"/>. Use this to check to check
    /// that the expected message is published only once.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to look for.</typeparam>
    /// <param name="expression">An expression to filter messages by.</param>
    /// <returns>The number of messages matched by the <paramref name="predicate"/>.</returns>
    public int CountMessagesPublished<TMessage>()
    {
        return CountMessagesPublished<TMessage>(null);
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
    public int CountDispatchedMail(string toEmail, params string[] textPartsToMatch)
    {
        if (_serviceScope.GetService<IMailDispatchSession>() is not AuditableMailDispatchSession auditableMailDispatchSession)
        {
            throw new Exception($"{nameof(IMailDispatchSession)} is expected to be an instance of {nameof(AuditableMailDispatchSession)} in testing");
        }

        return auditableMailDispatchSession.CountDispatched(toEmail, textPartsToMatch);
    }
}
