namespace Cofoundry.Domain.Data;

/// <summary>
/// <para>
/// Authorized tasks represent a single user-based operation that can be executed without
/// being logged in. Task authorization is validated by a crytographically secure token, 
/// often communicated via an out-of-band communication mechanism such as an email. Examples 
/// include password reset or email address validation flows.
/// </para>
/// <para>
/// Tasks tend to be single-use and can be marked when completed, and can also be 
/// invalidated explicitly. They can also be rate-limited by IPAddress and time-limited
/// by validating against the <see cref="CreateDate"/>.
/// </para>
/// </summary>
public class AuthorizedTask
{
    /// <summary>
    /// Primary key and unique identifier for the task, which is combined
    /// with the <see cref="AuthorizationCode"/> to create the unique token that
    /// is used to identify and authorize the task execution via a url or similar mechanism.
    /// </summary>
    public Guid AuthorizedTaskId { get; set; }

    /// <summary>
    /// The primary key of the user associated with the task e.g. for an acount recovery task
    /// this is the user account being recovered.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The user associated with the task e.g. for an acount recovery task this
    /// is the user account being recovered.
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// A unique 6-character code used to group tasks by. These codes are associated with an
    /// <see cref="IAuthorizedTaskTypeDefinition"/> implementation.
    /// </summary>
    public string AuthorizedTaskTypeCode { get; set; }

    /// <summary>
    /// A cryptographically strong random code that is used to authenticate before 
    /// the action is permitted to be executed.
    /// </summary>
    public string AuthorizationCode { get; set; }

    /// <summary>
    /// The IPAddress of the client that initiated (authorized) the task request.
    /// </summary>
    public long? IPAddressId { get; set; }

    /// <summary>
    /// The IPAddress of the client that initiated (authorized) the task request.
    /// </summary>
    public IPAddress IPAddress { get; set; }

    /// <summary>
    /// Data to be included with the task. This might be data used to authorized the
    /// request or data to be used once the request is authorized. E.g. when verifying
    /// an email address we store the email being verified, this can then be used to 
    /// validate that the current email is still the email that originated the request.
    /// If we only wanted to change an email address after it has been vertified, then we could
    /// store the email in <see cref="TaskData"/>, and then once verified we can save
    /// the new email address to the user.
    /// </summary>
    public string TaskData { get; set; }

    /// <summary>
    /// The date the task was created. This is used to calculate the
    /// expiry date if an expiry window is specified during validation.
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// The date at which the task was marked invalid e.g. when a password is
    /// changed, any pending password reset tasks associated with the user are invalidated.
    /// </summary>
    public DateTime? InvalidatedDate { get; set; }

    /// <summary>
    /// The date at which the task will expire and automatically become invalid. This date
    /// is optional and if set to <see langword="null"/> then the task does not expire.
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// The date at which the task was completed e.g. for a password reset, this is when the password 
    /// has been changed. This will be <see langword="null"/> if the task has not been completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }
}
