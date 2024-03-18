﻿using System.Net;

namespace Cofoundry.Core.Mail.Internal;

/// <summary>
/// Renders the contents of a mail template to html/text strings in preparation for 
/// sending out as an email.
/// </summary>
public class MailMessageRenderer : IMailMessageRenderer
{
    private readonly IMailViewRenderer _viewRenderer;

    public MailMessageRenderer(
        IMailViewRenderer viewRenderer
        )
    {
        _viewRenderer = viewRenderer;
    }

    /// <summary>
    ///  Renders the contents of a mail template and formats it into a MailMessage
    ///  object that can be used to send out an email.
    /// </summary>
    /// <param name="template">The mail template that describes the data and template information for the email</param>
    /// <param name="toAddress">The address to send the email to.</param>
    /// <returns>Formatted MailMessage</returns>
    public async Task<MailMessage> RenderAsync(IMailTemplate template, MailAddress toAddress)
    {
        var message = new MailMessage()
        {
            Subject = template.Subject,
            To = toAddress
        };

        FormatFromAddress(template, message);

        var textBody = await RenderViewAsync(template, "text");

        if (!string.IsNullOrWhiteSpace(textBody))
        {
            // Remove any html encodings created by the Razor parser
            message.TextBody = WebUtility.HtmlDecode(textBody);
        }
        message.HtmlBody = await RenderViewAsync(template, "html");

        if (message.HtmlBody == null && message.TextBody == null)
        {
            throw new InvalidOperationException($"Couldn't find a text or html mail template file for '{template.ViewFile}'");
        }

        return message;
    }

    private void FormatFromAddress(IMailTemplate template, MailMessage message)
    {
        if (template is IMailTemplateWithCustomFromAddress mailTemplateWithCustomFromAddress)
        {
            message.From = mailTemplateWithCustomFromAddress.From;
        }
    }

    private async Task<string?> RenderViewAsync(IMailTemplate template, string type)
    {
        var path = string.Format("{0}_{1}.cshtml", template.ViewFile, type);
        string? view = null;

        try
        {
            view = await _viewRenderer.RenderAsync(path, template);
        }
        catch (Exception ex)
        {
            throw new TemplateRenderException(path, template, ex);
        }

        return view?.Trim();
    }
}
