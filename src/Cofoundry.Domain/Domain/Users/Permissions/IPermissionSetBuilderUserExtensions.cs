using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// <see cref="IPermissionSetBuilder"/> extension methods for users.
/// </summary>
public static class IPermissionSetBuilderUserExtensions
{
    extension(IPermissionSetBuilder builder)
    {
        /// <summary>
        /// Removes permissions for managing user accounts in all user areas, however this does not
        /// exclude permissions for managing the currently signed in user account.
        /// </summary>
        public IPermissionSetBuilder ExcludeUserInAllUserAreas()
        {
            return builder.Exclude(permissions => permissions.ExceptUserManagementPermissions());
        }

        /// <summary>
        /// Configure the builder to include all permissions for users in the Cofoundry admin user area.
        /// </summary>
        public IPermissionSetBuilder IncludeUserInCofoundryAdminUserArea()
        {
            return Run(builder, configure, true);

            static void configure(CofoundryUserPermissionBuilder c) => c.All();
        }

        /// <summary>
        /// Configure the builder to include permissions for users in the Cofoundry admin user area.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public IPermissionSetBuilder IncludeUserInCofoundryAdminUserArea(Action<CofoundryUserPermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for users in the Cofoundry admin user area
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public IPermissionSetBuilder ExcludeUserInCofoundryAdminUserArea(Action<CofoundryUserPermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for users in the Cofoundry admin user area.
        /// </summary>
        public IPermissionSetBuilder ExcludeUserInCofoundryAdminUserArea()
        {
            return Run(builder, configure, false);

            static void configure(CofoundryUserPermissionBuilder c) => c.All();
        }

        /// <summary>
        /// Configure the builder to include all permissions for users in a custom user area (excludes the Cofoundry admin user area).
        /// </summary>
        public IPermissionSetBuilder IncludeUserInCustomUserArea()
        {
            return Run(builder, configure, true);

            static void configure(NonCofoundryUserPermissionBuilder c) => c.All();
        }

        /// <summary>
        /// Configure the builder to include permissions for users in a custom user area (excludes the Cofoundry admin user area).
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public IPermissionSetBuilder IncludeUserInCustomUserArea(Action<NonCofoundryUserPermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for users in a custom user area (excludes the Cofoundry admin user area).
        /// </summary>
        public IPermissionSetBuilder ExcludeUserInCustomUserArea()
        {
            return Run(builder, configure, false);

            static void configure(NonCofoundryUserPermissionBuilder c) => c.All();
        }

        /// <summary>
        /// Configure the builder to exclude permissions for users in a custom user area (excludes the Cofoundry admin user area).
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public IPermissionSetBuilder ExcludeUserInCustomUserArea(Action<NonCofoundryUserPermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }

        /// <summary>
        /// Configure the builder to include all permissions for the currently signed in user account.
        /// </summary>
        public IPermissionSetBuilder IncludeCurrentUser()
        {
            return Run(builder, configure, true);

            static void configure(CurrentUserPermissionBuilder c) => c.All();
        }

        /// <summary>
        /// Configure the builder to include permissions for the currently signed in user account.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public IPermissionSetBuilder IncludeCurrentUser(Action<CurrentUserPermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for the currently signed in user account.
        /// </summary>
        public IPermissionSetBuilder ExcludeCurrentUser()
        {
            return Run(builder, configure, false);

            static void configure(CurrentUserPermissionBuilder c) => c.All();
        }

        /// <summary>
        /// Configure the builder to exclude permissions for the currently signed in user account.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public IPermissionSetBuilder ExcludeCurrentUser(Action<CurrentUserPermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<CofoundryUserPermissionBuilder> configure, bool isIncludeOperation)
    {
        var usersBuilder = new CofoundryUserPermissionBuilder(builder, isIncludeOperation);
        configure(usersBuilder);

        return builder;
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<NonCofoundryUserPermissionBuilder> configure, bool isIncludeOperation)
    {
        var usersBuilder = new NonCofoundryUserPermissionBuilder(builder, isIncludeOperation);
        configure(usersBuilder);

        return builder;
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<CurrentUserPermissionBuilder> configure, bool isIncludeOperation)
    {
        var usersBuilder = new CurrentUserPermissionBuilder(builder, isIncludeOperation);
        configure(usersBuilder);

        return builder;
    }
}
