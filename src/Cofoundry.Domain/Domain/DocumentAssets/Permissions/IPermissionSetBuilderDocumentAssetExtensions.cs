using Cofoundry.Domain.Internal;
using System;

namespace Cofoundry.Domain
{
    public static class IPermissionSetBuilderDocumentAssetExtensions
    {
        /// <summary>
        /// Configure the builder to include all permissions for document assets.
        /// </summary>
        public static IPermissionSetBuilder IncludeDocumentAsset(this IPermissionSetBuilder builder)
        {
            return Run(builder, null, true);
        }

        /// <summary>
        /// Configure the builder to include permissions for document assets.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public static IPermissionSetBuilder IncludeDocumentAsset(this IPermissionSetBuilder builder, Action<DocumentAssetPermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for document assets.
        /// </summary>
        public static IPermissionSetBuilder ExcludeDocumentAsset(this IPermissionSetBuilder builder)
        {
            return Run(builder, null, false);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for document assets.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public static IPermissionSetBuilder ExcludeDocumentAsset(this IPermissionSetBuilder builder, Action<DocumentAssetPermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }

        private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<DocumentAssetPermissionBuilder> configure, bool isIncludeOperation)
        {
            if (configure == null) configure = c => c.All();
            var entityBuilder = new DocumentAssetPermissionBuilder(builder, isIncludeOperation);
            configure(entityBuilder);

            return builder;
        }
    }
}