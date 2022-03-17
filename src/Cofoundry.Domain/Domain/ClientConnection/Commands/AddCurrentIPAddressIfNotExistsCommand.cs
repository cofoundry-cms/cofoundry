namespace Cofoundry.Domain;

/// <summary>
/// Adds the IP Address of the currently logged in user to the 
/// </summary>
public class AddCurrentIPAddressIfNotExistsCommand : ICommand
{
    [OutputValue]
    public long? OutputIPAddressId { get; set; }
}
