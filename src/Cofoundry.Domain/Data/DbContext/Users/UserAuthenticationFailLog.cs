namespace Cofoundry.Domain.Data;

/// <summary>
/// A logging table that record failed user authentication attempts.
/// </summary>
public class UserAuthenticationFailLog
{
    /// <summary>
    /// Database id of the <see cref="UserAuthenticationFailLog"/>.
    /// </summary>
    public long UserAuthenticationFailLogId { get; set; }

    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> that the client was attempting to
    /// authenticated against.
    /// </summary>
    public string UserAreaCode { get; set; } = string.Empty;

    private UserArea? _userArea;
    /// <summary>
    /// The <see cref="Data.UserArea"/> that the client was attempting to
    /// authenticated against.
    /// </summary>
    public virtual UserArea UserArea
    {
        get => _userArea ?? throw NavigationPropertyNotInitializedException.Create<UserAuthenticationFailLog>(nameof(UserArea));
        set => _userArea = value;
    }

    /// <summary>
    /// The username parameter of the authentication request. This is the
    /// normalized and uniquified version of the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// IP Address of the connection requested authentication.
    /// </summary>
    public long IPAddressId { get; set; }

    private IPAddress? _ipAddress;
    /// <summary>
    /// IP Address of the connection requested authentication.
    /// </summary>
    public IPAddress IPAddress
    {
        get => _ipAddress ?? throw NavigationPropertyNotInitializedException.Create<UserAuthenticationFailLog>(nameof(IPAddress));
        set => _ipAddress = value;
    }

    /// <summary>
    /// The date and time of the authentication failure event.
    /// </summary>
    public DateTime CreateDate { get; set; }
}
