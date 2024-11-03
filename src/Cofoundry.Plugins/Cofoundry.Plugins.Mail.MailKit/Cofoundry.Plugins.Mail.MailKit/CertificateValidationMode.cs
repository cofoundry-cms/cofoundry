namespace Cofoundry.Plugins.Mail.MailKit;

/// <summary>
/// Used to indicate the rules to use for smtp server ssl certificate validation.
/// </summary>
public enum CertificateValidationMode
{
    /// <summary>
    /// Uses the default MailKit validator, which allows valid certificates
    /// and self-signed certificates with an untrusted root.
    /// </summary>
    /// <remarks>
    /// See https://github.com/jstedfast/MailKit/issues/307
    /// </remarks>
    Default,

    /// <summary>
    /// Ignore certificate errors.
    /// </summary>
    All,

    /// <summary>
    /// Allows only valid certificates without errors.
    /// </summary>
    ValidOnly
}
