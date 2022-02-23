using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
    /// include or exclude document asset permissions.
    /// </summary>
    public class DocumentAssetPermissionBuilder : EntityPermissionBuilder
    {
        public DocumentAssetPermissionBuilder(
            IPermissionSetBuilder permissionSetBuilder,
            bool isIncludeOperation
            )
            : base(permissionSetBuilder, isIncludeOperation)
        {
        }

        /// <summary>
        /// All permissions, including CRUD and admin module permissions.
        /// </summary>
        public DocumentAssetPermissionBuilder All()
        {
            return CRUD().AdminModule();
        }

        /// <summary>
        /// Create, Read, Update and Delete permissions.
        /// </summary>
        public DocumentAssetPermissionBuilder CRUD()
        {
            return Create().Read().Update().Delete();
        }

        /// <summary>
        /// Read access to document assets. Read access is required in order
        /// to include any other document asset permissions.
        /// </summary>
        public DocumentAssetPermissionBuilder Read()
        {
            Assign<DocumentAssetReadPermission>();
            return this;
        }

        /// <summary>
        /// Permission to create new document assets.
        /// </summary>
        public DocumentAssetPermissionBuilder Create()
        {
            Assign<DocumentAssetCreatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to update a document asset.
        /// </summary>
        public DocumentAssetPermissionBuilder Update()
        {
            Assign<DocumentAssetUpdatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to delete a document asset.
        /// </summary>
        public DocumentAssetPermissionBuilder Delete()
        {
            Assign<DocumentAssetDeletePermission>();
            return this;
        }

        /// <summary>
        /// Permission to access the document assets module in the admin panel.
        /// </summary>
        public DocumentAssetPermissionBuilder AdminModule()
        {
            Assign<DocumentAssetAdminModulePermission>();
            return this;
        }
    }
}