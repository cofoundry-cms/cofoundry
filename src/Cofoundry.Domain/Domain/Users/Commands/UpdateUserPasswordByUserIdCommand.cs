using System.Runtime.Serialization;

namespace Cofoundry.Domain;

/// <summary>
/// Updates the password of a user to a specific value. Generally a user shouldn't
/// be able set another users password explicity, but this command is provided for
/// scenarios where authorization happpens through another mechanism such as via
/// <see cref="UpdateUserPasswordByCredentialsCommand"/>.
/// </summary>
public class UpdateUserPasswordByUserIdCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// Id of the user to update a password for.
    /// </summary>
    [Required]
    [PositiveInteger]
    public int UserId { get; set; }

    /// <summary>
    /// The value to set as the new account password. The password will go through additional validation depending 
    /// on the password policy configuration.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [IgnoreDataMember]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string NewPassword { get; set; }
}
