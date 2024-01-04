namespace Cofoundry.Domain;

/// <summary>
/// IP address constants.
/// </summary>
public interface IPAddressConstants
{
    /// <summary>
    /// A default value to use in IP Address logging, which typically
    /// indicates a non-network request from the local machine. The value
    /// "0.0.0.0" is used which is distinct from localhost but has various
    /// interpretations such as local machine or unknown.
    /// </summary>
    public const string Default = "0.0.0.0";
}
