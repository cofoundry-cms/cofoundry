using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// <see cref="IPermissionSetBuilder"/> extension methods for roles.
/// </summary>
public static class IPermissionSetBuilderRoleExtensions
{
    extension(IPermissionSetBuilder builder)
    {
        /// <summary>
        /// Configure the builder to include all permissions for roles.
        /// </summary>
        public IPermissionSetBuilder IncludeRole()
        {
            return Run(builder, null, true);
        }

        /// <summary>
        /// Configure the builder to include permissions for roles.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public IPermissionSetBuilder IncludeRole(Action<RolePermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for roles.
        /// </summary>
        public IPermissionSetBuilder ExcludeRole()
        {
            return Run(builder, null, false);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for roles.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public IPermissionSetBuilder ExcludeRole(Action<RolePermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<RolePermissionBuilder>? configure, bool isIncludeOperation)
    {
        if (configure == null)
        {
            configure = c => c.All();
        }

        var entityBuilder = new RolePermissionBuilder(builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }
}
