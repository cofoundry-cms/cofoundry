﻿namespace Cofoundry.Domain;

/// <summary>
/// Returns <see langword="true"/> if the parameters of the authentication
/// attempt exceed the limits sets in configuration e.g. attempts per IP Address
/// or per username.
/// </summary>
public class HasExceededMaxAuthenticationAttemptsQuery : IQuery<bool>
{
    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area 
    /// being authenticated.
    /// </summary>
    [Required]
    [StringLength(3)]
    public string UserAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// The username to query. This is expected to be in a "uniquified" 
    /// format, as this should have been already processed whenever this
    /// needs to be called.
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;
}
