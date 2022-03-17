namespace Cofoundry.Core.Mail;

/// <summary>
/// .Net core compatible version of System.Net.Mail.MailAddress
/// </summary>
public class MailAddress
{
    public MailAddress()
    {
    }

    public MailAddress(string address)
    {
        Address = address;
    }

    public MailAddress(string address, string displayName)
    {
        Address = address;
        DisplayName = displayName;
    }

    /// <summary>
    /// E-mail address e.g. 'john.smith@gmail.com'
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// User friendly display name e.g. 'John Smith'
    /// </summary>
    public string DisplayName { get; set; }

    public override string ToString()
    {
        return Address;
    }
}