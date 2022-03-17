using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
/// include or exclude page directory permissions.
/// </summary>
public class PageDirectoryPermissionBuilder : EntityPermissionBuilder
{
    public PageDirectoryPermissionBuilder(
        IPermissionSetBuilder permissionSetBuilder,
        bool isIncludeOperation
        )
        : base(permissionSetBuilder, isIncludeOperation)
    {
    }

    /// <summary>
    /// All permissions, including CRUD, special and admin module permissions.
    /// </summary>
    public PageDirectoryPermissionBuilder All()
    {
        return CRUD().Special().AdminModule();
    }

    /// <summary>
    /// Create, Read, Update and Delete permissions.
    /// </summary>
    public PageDirectoryPermissionBuilder CRUD()
    {
        return Create().Read().Update().Delete();
    }

    /// <summary>
    /// Special permissions: UpdateUrl and ManageAccessRules.
    /// </summary>
    public PageDirectoryPermissionBuilder Special()
    {
        return UpdateUrl().ManageAccessRules();
    }

    /// <summary>
    /// Read access to page directories. Read access is required in order
    /// to include any other page directory permissions.
    /// </summary>
    public PageDirectoryPermissionBuilder Read()
    {
        Assign<PageDirectoryReadPermission>();
        return this;
    }

    /// <summary>
    /// Permission to create new page directories.
    /// </summary>
    public PageDirectoryPermissionBuilder Create()
    {
        Assign<PageDirectoryCreatePermission>();
        return this;
    }

    /// <summary>
    /// Permission to update a page directory, but not to update the url path.
    /// </summary>
    public PageDirectoryPermissionBuilder Update()
    {
        Assign<PageDirectoryUpdatePermission>();
        return this;
    }

    /// <summary>
    /// Permission to delete a page directory.
    /// </summary>
    public PageDirectoryPermissionBuilder Delete()
    {
        Assign<PageDirectoryDeletePermission>();
        return this;
    }

    /// <summary>
    /// Permission to access the page directories module in the admin panel.
    /// </summary>
    public PageDirectoryPermissionBuilder AdminModule()
    {
        Assign<PageDirectoryAdminModulePermission>();
        return this;
    }

    /// <summary>
    /// Permission to update the url of a page directory.
    /// </summary>
    public PageDirectoryPermissionBuilder UpdateUrl()
    {
        Assign<PageDirectoryUpdateUrlPermission>();
        return this;
    }

    /// <summary>
    /// Permission to manage the set of access rule that
    /// govern who has permission to view pages in a directory and what
    /// action should be taken if permission is denied.
    /// </summary>
    public PageDirectoryPermissionBuilder ManageAccessRules()
    {
        Assign<PageDirectoryAccessRuleManagePermission>();
        return this;
    }
}
