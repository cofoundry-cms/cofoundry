namespace Cofoundry.Domain;

/// <summary>
/// Extensions for <see cref="IPermission"/>.
/// </summary>
public static class IPermissionExtensions
{
    extension(IPermission permission)
    {
        /// <summary>
        /// Creates a unique code that represents this permission by combining the
        /// entity definition code and the permission code. This is useful when
        /// representing the permission as a unique string identifier.
        /// </summary>
        /// <returns>A string identifier that uniquely identifies this permission.</returns>
        public string GetUniqueIdentifier()
        {
            return PermissionIdentifierFormatter.GetUniqueIdentifier(permission);
        }
    }
}
