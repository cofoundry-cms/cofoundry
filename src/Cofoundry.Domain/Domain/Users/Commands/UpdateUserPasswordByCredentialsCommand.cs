﻿using System.Runtime.Serialization;

namespace Cofoundry.Domain;

/// <summary>
/// Updates the password of an unathenticated user, using the
/// credentials in the command to authenticate the request.
/// </summary>
public class UpdateUserPasswordByCredentialsCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// The unique code of the user area the user is expected to belong to. Note
    /// that usernames are unique per user area and therefore a given username
    /// may have an account for more than one user area.
    /// </summary>
    [Required]
    public string UserAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// The users username, which is required to authenticate
    /// the user before performing the update.
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The users existing password. This is required to authenticate
    /// the user before performing the update.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [IgnoreDataMember]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// The value to set as the new account password. The password will go through additional validation depending 
    /// on the password policy configuration.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [IgnoreDataMember]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// The database id of the updated user. This is set after the command
    /// has been run.
    /// </summary>
    [OutputValue]
    public int OutputUserId { get; set; }
}
