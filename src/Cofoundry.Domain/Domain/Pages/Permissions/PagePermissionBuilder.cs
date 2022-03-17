using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
/// include or exclude page permissions.
/// </summary>
public class PagePermissionBuilder : EntityPermissionBuilder
{
    public PagePermissionBuilder(
        IPermissionSetBuilder permissionSetBuilder,
        bool isIncludeOperation
        )
        : base(permissionSetBuilder, isIncludeOperation)
    {
    }

    /// <summary>
    /// All permissions, including CRUD, special and admin module permissions.
    /// </summary>
    public PagePermissionBuilder All()
    {
        return CRUD().Special().AdminModule();
    }

    /// <summary>
    /// Create, Read, Update and Delete permissions.
    /// </summary>
    public PagePermissionBuilder CRUD()
    {
        return Create().Read().Update().Delete();
    }

    /// <summary>
    /// Special permissions: Publish, UpdateUrl and ManageAccessRules.
    /// </summary>
    public PagePermissionBuilder Special()
    {
        return Publish().UpdateUrl().ManageAccessRules();
    }

    /// <summary>
    /// Read access to pages. Read access is required in order
    /// to include any other page permissions.
    /// </summary>
    public PagePermissionBuilder Read()
    {
        Assign<PageReadPermission>();
        return this;
    }

    /// <summary>
    /// Permission to create new pages.
    /// </summary>
    public PagePermissionBuilder Create()
    {
        Assign<PageCreatePermission>();
        return this;
    }

    /// <summary>
    /// Permission to update a page, but not to update a url
    /// or publish.
    /// </summary>
    public PagePermissionBuilder Update()
    {
        Assign<PageUpdatePermission>();
        return this;
    }

    /// <summary>
    /// Permission to delete a page.
    /// </summary>
    public PagePermissionBuilder Delete()
    {
        Assign<PageDeletePermission>();
        return this;
    }

    /// <summary>
    /// Permission to access the pages module in the admin panel.
    /// </summary>
    public PagePermissionBuilder AdminModule()
    {
        Assign<PageAdminModulePermission>();
        return this;
    }

    /// <summary>
    /// Permission to publish and unpublish a page.
    /// </summary>
    public PagePermissionBuilder Publish()
    {
        Assign<PagePublishPermission>();
        return this;
    }

    /// <summary>
    /// Permission to update the url of a page.
    /// </summary>
    public PagePermissionBuilder UpdateUrl()
    {
        Assign<PageUpdateUrlPermission>();
        return this;
    }

    /// <summary>
    /// Permission to manage the set of access rule that
    /// govern who has permission to view a page and what
    /// action should be taken if permission is denied.
    /// </summary>
    public PagePermissionBuilder ManageAccessRules()
    {
        Assign<PageAccessRuleManagePermission>();
        return this;
    }
}
