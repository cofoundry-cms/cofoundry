using Cofoundry.Domain.Data;

namespace Cofoundry.Domain;

/// <summary>
/// Used to generate the security stamp that is assigned to the
/// <see cref="User.SecurityStamp"/> property. For more information
/// on the purpose of this property, read the <see cref="User.SecurityStamp"/>
/// documentation.
/// </summary>
public interface ISecurityStampGenerator
{
    /// <summary>
    /// Generates a cryptographically random set of data that
    /// can be used as the security stamp assigned to a user.
    /// By default the stamp is 160-bits encoded into Base32.
    /// </summary>
    string Generate();
}
