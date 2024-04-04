using Cofoundry.Core.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Default implementation of <see cref="ISecurityStampGenerator"/>.
/// </summary>
/// <remarks>
/// This implementation mirrors the logic in the private NewSecurityStamp method in
/// <see cref="Microsoft.AspNetCore.Identity.UserManager{TUser}"/>.
/// See https://github.com/dotnet/aspnetcore/blob/v8.0.0/src/Identity/Extensions.Core/src/UserManager.cs
/// for source.
/// </remarks>
public class SecurityStampGenerator : ISecurityStampGenerator
{
    /// <inheritdoc/>
    public string Generate()
    {
        return Base32Converter.GenerateBase32();
    }
}
