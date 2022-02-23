using Cofoundry.Domain.Internal;
using System;

namespace Cofoundry.Domain
{
    public static class IPermissionSetBuilderPageExtensions
    {
        /// <summary>
        /// Configure the builder to include all permissions for pages.
        /// </summary>
        public static IPermissionSetBuilder IncludePage(this IPermissionSetBuilder builder)
        {
            return Run(builder, null, true);
        }

        /// <summary>
        /// Configure the builder to include permissions for pages.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public static IPermissionSetBuilder IncludePage(this IPermissionSetBuilder builder, Action<PagePermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for pages.
        /// </summary>
        public static IPermissionSetBuilder ExcludePage(this IPermissionSetBuilder builder)
        {
            return Run(builder, null, false);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for pages.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public static IPermissionSetBuilder ExcludePage(this IPermissionSetBuilder builder, Action<PagePermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }

        private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<PagePermissionBuilder> configure, bool isIncludeOperation)
        {
            if (configure == null) configure = c => c.All();
            var entityBuilder = new PagePermissionBuilder(builder, isIncludeOperation);
            configure(entityBuilder);

            return builder;
        }
    }
}