using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
    /// include or exclude image asset permissions.
    /// </summary>
    public class ImageAssetPermissionBuilder : EntityPermissionBuilder
    {
        public ImageAssetPermissionBuilder(
            IPermissionSetBuilder permissionSetBuilder,
            bool isIncludeOperation
            )
            : base(permissionSetBuilder, isIncludeOperation)
        {
        }

        /// <summary>
        /// All permissions, including CRUD and admin module permissions.
        /// </summary>
        public ImageAssetPermissionBuilder All()
        {
            return CRUD().AdminModule();
        }

        /// <summary>
        /// Create, Read, Update and Delete permissions.
        /// </summary>
        public ImageAssetPermissionBuilder CRUD()
        {
            return Create().Read().Update().Delete();
        }

        /// <summary>
        /// Read access to image assets. Read access is required in order
        /// to include any other image asset permissions.
        /// </summary>
        public ImageAssetPermissionBuilder Read()
        {
            Assign<ImageAssetReadPermission>();
            return this;
        }

        /// <summary>
        /// Permission to create new image assets.
        /// </summary>
        public ImageAssetPermissionBuilder Create()
        {
            Assign<ImageAssetCreatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to update an image asset.
        /// </summary>
        public ImageAssetPermissionBuilder Update()
        {
            Assign<ImageAssetUpdatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to delete an image asset.
        /// </summary>
        public ImageAssetPermissionBuilder Delete()
        {
            Assign<ImageAssetDeletePermission>();
            return this;
        }

        /// <summary>
        /// Permission to access the image assets module in the admin panel.
        /// </summary>
        public ImageAssetPermissionBuilder AdminModule()
        {
            Assign<ImageAssetAdminModulePermission>();
            return this;
        }
    }
}