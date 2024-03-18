﻿using Cofoundry.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core.Mail;

/// <summary>
/// Generic settings for sending mail.
/// </summary>
public class MailSettings : CofoundryConfigurationSettingsBase
{
    /// <summary>
    /// Indicates whether emails should be sent and how. The default 
    /// value is LocalDrop, so you should set this to Send in production
    /// deployments
    /// </summary>
    public MailSendMode SendMode { get; set; }

    /// <summary>
    /// An email address to redirect all mail to when using MailSendMode.SendToDebugAddress
    /// </summary>
    public string? DebugEmailAddress { get; set; }

    /// <summary>
    /// The default address to send emails. To override this your
    /// template should implement IMailTemplateWithCustomFromAddress
    /// </summary>
    [Required]
    public string DefaultFromAddress { get; set; } = "auto@cofoundry.org";

    /// <summary>
    /// Optionally the name to display with the default From Address
    /// </summary>
    public string? DefaultFromAddressDisplayName { get; set; }

    /// <summary>
    /// The path to the folder to save mail to when using SendMode.LocalDrop. Defaults 
    /// to ~/App_Data/Emails
    /// </summary>
    [Required]
    public string MailDropDirectory { get; set; } = "~/App_Data/Emails";
}
