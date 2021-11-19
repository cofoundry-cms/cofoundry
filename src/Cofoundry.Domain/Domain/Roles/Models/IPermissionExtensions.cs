namespace Cofoundry.Domain
{
    public static class IPermissionExtensions
    {
        /// <summary>
        /// Creates a unique code that represents this permission by combining the
        /// entity definition code and the permission code. This is useful when
        /// representing the permission as a unique string identifier.
        /// </summary>
        /// <param name="permission">Permission to get the unique code for.</param>
        /// <returns>A string identifier that uniquely identifies this permission.</returns>
        public static string GetUniqueIdentifier(this IPermission permission)
        {
            return PermissionIdentifierFormatter.GetUniqueIdentifier(permission);
        }
    }
}
