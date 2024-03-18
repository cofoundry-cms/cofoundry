﻿using System.Runtime.Serialization;

namespace Cofoundry.Domain;

/// <summary>
/// Updates the password of the currently signed in user, using the
/// <see cref="UpdateCurrentUserPasswordCommand.OldPassword"/> field 
/// to authenticate the request.
/// </summary>
public class UpdateCurrentUserPasswordCommand : ICommand, ILoggableCommand
{
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
}
