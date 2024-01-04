namespace Cofoundry.Domain.Data;

/// <summary>
/// A logging table that record successful user authentication events.
/// </summary>
public class UserAuthenticationLog
{
    /// <summary>
    /// Database id of the <see cref="UserAuthenticationLog"/>.
    /// </summary>
    public long UserAuthenticationLogId { get; set; }

    /// <summary>
    /// The <see cref="User.UserId"/> of the user that successfully authenticated.
    /// </summary>
    public int UserId { get; set; }

    private User? _user;
    /// <summary>
    /// The <see cref="Data.User"/> that successfully authenticated.
    /// </summary>
    public virtual User User
    {
        get => _user ?? throw NavigationPropertyNotInitializedException.Create<UserAuthenticationLog>(nameof(User));
        set => _user = value;
    }

    /// <summary>
    /// IP Address of the connection that authenticated the user.
    /// </summary>
    public long IPAddressId { get; set; }

    private IPAddress? _ipAddress;
    /// <summary>
    /// IP Address of the connection that authenticated the user.
    /// </summary>
    public IPAddress IPAddress
    {
        get => _ipAddress ?? throw NavigationPropertyNotInitializedException.Create<UserAuthenticationLog>(nameof(IPAddress));
        set => _ipAddress = value;
    }

    /// <summary>
    /// The date and time of the authentication event.
    /// </summary>
    public DateTime CreateDate { get; set; }
}
