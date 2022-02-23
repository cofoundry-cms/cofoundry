using Cofoundry.Domain.Extendable;
using System;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
    /// include or exclude custom entity permissions.
    /// </summary>
    public class CustomEntityPermissionBuilder : EntityPermissionBuilder
    {
        private readonly ICustomEntityDefinition _customEntityDefinition;

        public CustomEntityPermissionBuilder(
            ICustomEntityDefinition customEntityDefinition,
            IPermissionSetBuilder permissionSetBuilder,
            bool isIncludeOperation
            )
            : base(permissionSetBuilder, isIncludeOperation)
        {
            if (customEntityDefinition == null) throw new ArgumentNullException(nameof(customEntityDefinition));
            _customEntityDefinition = customEntityDefinition;
        }

        /// <summary>
        /// All permissions, including CRUD, special and admin module permissions.
        /// </summary>
        public CustomEntityPermissionBuilder All()
        {
            return CRUD().Special().AdminModule();
        }

        /// <summary>
        /// Create, Read, Update and Delete permissions.
        /// </summary>
        public CustomEntityPermissionBuilder CRUD()
        {
            return Create().Read().Update().Delete();
        }

        /// <summary>
        /// Special permissions: Publish and UpdateUrl.
        /// </summary>
        public CustomEntityPermissionBuilder Special()
        {
            return Publish().UpdateUrl();
        }

        /// <summary>
        /// Read access to a custom entity. Read access is required in order
        /// to include any other custom entity permissions.
        /// </summary>
        public CustomEntityPermissionBuilder Read()
        {
            Assign(new CustomEntityReadPermission(_customEntityDefinition));
            return this;
        }

        /// <summary>
        /// Permission to create new custom entities.
        /// </summary>
        public CustomEntityPermissionBuilder Create()
        {
            Assign(new CustomEntityCreatePermission(_customEntityDefinition));
            return this;
        }

        /// <summary>
        /// Permission to update a custom entity, but not to update a url
        /// or publish.
        /// </summary>
        public CustomEntityPermissionBuilder Update()
        {
            Assign(new CustomEntityUpdatePermission(_customEntityDefinition));
            return this;
        }

        /// <summary>
        /// Permission to delete a custom entity.
        /// </summary>
        public CustomEntityPermissionBuilder Delete()
        {
            Assign(new CustomEntityDeletePermission(_customEntityDefinition));
            return this;
        }

        /// <summary>
        /// Permission to access the custom entity module in the admin panel.
        /// </summary>
        public CustomEntityPermissionBuilder AdminModule()
        {
            Assign(new CustomEntityAdminModulePermission(_customEntityDefinition));
            return this;
        }

        /// <summary>
        /// Permission to publish and unpublish a custom entity.
        /// </summary>
        public CustomEntityPermissionBuilder Publish()
        {
            Assign(new CustomEntityPublishPermission(_customEntityDefinition));
            return this;
        }

        /// <summary>
        /// Permission to update the UrlSlug and locale of a custom entity which often forms
        /// the identity of the entity and can form part of the URL when used in
        /// custom entity pages.
        /// </summary>
        public CustomEntityPermissionBuilder UpdateUrl()
        {
            Assign(new CustomEntityUpdateUrlPermission(_customEntityDefinition));
            return this;
        }
    }
}