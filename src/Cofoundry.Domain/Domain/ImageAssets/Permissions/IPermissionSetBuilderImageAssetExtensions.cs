using Cofoundry.Domain.Internal;
using System;

namespace Cofoundry.Domain
{
    public static class IPermissionSetBuilderImageAssetExtensions
    {
        /// <summary>
        /// Configure the builder to include all permissions for image assets.
        /// </summary>
        public static IPermissionSetBuilder IncludeImageAsset(this IPermissionSetBuilder builder)
        {
            return Run(builder, null, true);
        }

        /// <summary>
        /// Configure the builder to include permissions for image assets.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public static IPermissionSetBuilder IncludeImageAsset(this IPermissionSetBuilder builder, Action<ImageAssetPermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for image assets.
        /// </summary>
        public static IPermissionSetBuilder ExcludeImageAsset(this IPermissionSetBuilder builder)
        {
            return Run(builder, null, false);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for image assets.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public static IPermissionSetBuilder ExcludeImageAsset(this IPermissionSetBuilder builder, Action<ImageAssetPermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }

        private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<ImageAssetPermissionBuilder> configure, bool isIncludeOperation)
        {
            if (configure == null) configure = c => c.All();
            var entityBuilder = new ImageAssetPermissionBuilder(builder, isIncludeOperation);
            configure(entityBuilder);

            return builder;
        }
    }
}