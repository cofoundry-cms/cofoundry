using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for image assets.
/// </summary>
public static class IPermissionSetBuilderImageAssetExtensions
{
    extension(IPermissionSetBuilder builder)
    {
        /// <summary>
        /// Configure the builder to include all permissions for image assets.
        /// </summary>
        public IPermissionSetBuilder IncludeImageAsset()
        {
            return Run(builder, null, true);
        }

        /// <summary>
        /// Configure the builder to include permissions for image assets.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public IPermissionSetBuilder IncludeImageAsset(Action<ImageAssetPermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for image assets.
        /// </summary>
        public IPermissionSetBuilder ExcludeImageAsset()
        {
            return Run(builder, null, false);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for image assets.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public IPermissionSetBuilder ExcludeImageAsset(Action<ImageAssetPermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<ImageAssetPermissionBuilder>? configure, bool isIncludeOperation)
    {
        if (configure == null)
        {
            configure = c => c.All();
        }

        var entityBuilder = new ImageAssetPermissionBuilder(builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }
}
