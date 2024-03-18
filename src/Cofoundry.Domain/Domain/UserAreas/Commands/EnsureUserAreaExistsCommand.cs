﻿namespace Cofoundry.Domain;

/// <summary>
/// User area definitions are definied in code and stored in the database, so if they are missing
/// from the databse we need to add them. Execute this to ensure that the definition has been saved
/// to the database before assigning it to another entity.
/// </summary>
public class EnsureUserAreaExistsCommand : ICommand
{
    public EnsureUserAreaExistsCommand()
    {
    }

    public EnsureUserAreaExistsCommand(string userAreaCode)
    {
        UserAreaCode = userAreaCode;
    }

    [Required]
    [MaxLength(3)]
    public string UserAreaCode { get; set; } = string.Empty;
}
