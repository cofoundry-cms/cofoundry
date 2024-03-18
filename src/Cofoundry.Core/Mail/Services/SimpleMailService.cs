﻿namespace Cofoundry.Core.Mail.Internal;

/// <summary>
/// A service to for sending email directly using the IMailClient implementation. Does not support
/// batch (high volumne), queuing or background sending.
/// </summary>
public class SimpleMailService : IMailService
{
    private readonly IMailDispatchService _mailDispatchService;
    private readonly IMailMessageRenderer _mailMessageRenderer;

    public SimpleMailService(
        IMailDispatchService mailDispatchService,
        IMailMessageRenderer mailMessageRenderer
        )
    {
        _mailDispatchService = mailDispatchService;
        _mailMessageRenderer = mailMessageRenderer;
    }

    public async Task SendAsync(string toEmail, string? toDisplayName, IMailTemplate template)
    {
        var toAddress = new MailAddress(toEmail, toDisplayName);
        var message = await _mailMessageRenderer.RenderAsync(template, toAddress);

        await _mailDispatchService.DispatchAsync(message);
    }

    public async Task SendAsync(string toEmail, IMailTemplate template)
    {
        await SendAsync(toEmail, null, template);
    }

    public async Task SendAsync(MailAddress address, IMailTemplate template)
    {
        ArgumentNullException.ThrowIfNull(address);

        await SendAsync(address.Address, address.DisplayName, template);
    }
}