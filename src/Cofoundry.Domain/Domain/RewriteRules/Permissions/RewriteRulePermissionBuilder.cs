using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
    /// include or exclude rewrite rule permissions.
    /// </summary>
    public class RewriteRulePermissionBuilder : EntityPermissionBuilder
    {
        public RewriteRulePermissionBuilder(
            IPermissionSetBuilder permissionSetBuilder,
            bool isIncludeOperation
            )
            : base(permissionSetBuilder, isIncludeOperation)
        {
        }

        /// <summary>
        /// All permissions, including CRUD and admin module permissions.
        /// </summary>
        public RewriteRulePermissionBuilder All()
        {
            return CRUD().AdminModule();
        }

        /// <summary>
        /// Create, Read, Update and Delete permissions.
        /// </summary>
        public RewriteRulePermissionBuilder CRUD()
        {
            return Create().Read().Update().Delete();
        }

        /// <summary>
        /// Read access to rewrite rules. Read access is required in order
        /// to include any other rewrite rule permissions.
        /// </summary>
        public RewriteRulePermissionBuilder Read()
        {
            Assign<RewriteRuleReadPermission>();
            return this;
        }

        /// <summary>
        /// Permission to create new rewrite rules.
        /// </summary>
        public RewriteRulePermissionBuilder Create()
        {
            Assign<RewriteRuleCreatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to update a rewrite rule.
        /// </summary>
        public RewriteRulePermissionBuilder Update()
        {
            Assign<RewriteRuleUpdatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to delete a rewrite rule.
        /// </summary>
        public RewriteRulePermissionBuilder Delete()
        {
            Assign<RewriteRuleDeletePermission>();
            return this;
        }

        /// <summary>
        /// Permission to access the rewrite rules module in the admin panel.
        /// </summary>
        public RewriteRulePermissionBuilder AdminModule()
        {
            Assign<RewriteRuleAdminModulePermission>();
            return this;
        }
    }
}