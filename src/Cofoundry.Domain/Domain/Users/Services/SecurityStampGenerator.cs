using System.Security.Cryptography;
using Cofoundry.Core.Internal;

namespace Cofoundry.Domain
{
    /// <inheritdoc/>
    /// <remarks>
    /// This implementation mirrors the logic in <see cref="UserManager.NewSecurityStamp"/>.
    /// See https://github.com/dotnet/aspnetcore/blob/v6.0.1/src/Identity/Extensions.Core/src/UserManager.cs
    /// for source.
    /// </remarks>
    public class SecurityStampGenerator : ISecurityStampGenerator
    {
        public string Generate()
        {
            byte[] bytes = new byte[20];
            RandomNumberGenerator.Fill(bytes);

            return Base32Converter.ToBase32(bytes);
        }
    }
}
