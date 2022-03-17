namespace Cofoundry.Domain.Data;

/// <summary>
/// A domain used as part of an email address i.e. the part after
/// the '@'. These are stored as "unique" email domains, whereby
/// multiple domains that resolve to the same mailboxes will only have
/// a single EmailDomain entry in the database.
/// </summary>
public class EmailDomain
{
    /// <summary>
    /// Database id of the record.
    /// </summary>
    public int EmailDomainId { get; set; }

    /// <summary>
    /// The name of the domain, which should be in a valid lowercased
    /// format e.g. "example.com" or "müller.example.com".
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// An unsalted SHA256 hash representing the unique name of the domain.
    /// The default implementation uses the <see cref="System.Uri.IdnHost"/> variation of 
    /// the domain when parsed by <see cref="System.Uri"/> to better support international 
    /// domain names.
    /// </summary>
    public byte[] NameHash { get; set; }

    /// <summary>
    /// Date and time at which the email domain record was added
    /// to the system.
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// Users with an email that belongs to this domain.
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
