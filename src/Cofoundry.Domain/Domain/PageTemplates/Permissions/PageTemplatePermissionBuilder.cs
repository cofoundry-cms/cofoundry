using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
/// include or exclude page template permissions.
/// </summary>
public class PageTemplatePermissionBuilder : EntityPermissionBuilder
{
    public PageTemplatePermissionBuilder(
        IPermissionSetBuilder permissionSetBuilder,
        bool isIncludeOperation
        )
        : base(permissionSetBuilder, isIncludeOperation)
    {
    }

    /// <summary>
    /// All permissions, including CRUD and admin module permissions.
    /// </summary>
    public PageTemplatePermissionBuilder All()
    {
        return CRUD().AdminModule();
    }

    /// <summary>
    /// Create, Read, Update and Delete permissions.
    /// </summary>
    public PageTemplatePermissionBuilder CRUD()
    {
        return Create().Read().Update().Delete();
    }

    /// <summary>
    /// Read access to page templates. Read access is required in order
    /// to include any other page template permissions.
    /// </summary>
    public PageTemplatePermissionBuilder Read()
    {
        Assign<PageTemplateReadPermission>();
        return this;
    }

    /// <summary>
    /// Permission to create new page templates.
    /// </summary>
    public PageTemplatePermissionBuilder Create()
    {
        Assign<PageTemplateCreatePermission>();
        return this;
    }

    /// <summary>
    /// Permission to update a page template.
    /// </summary>
    public PageTemplatePermissionBuilder Update()
    {
        Assign<PageTemplateUpdatePermission>();
        return this;
    }

    /// <summary>
    /// Permission to delete a page template.
    /// </summary>
    public PageTemplatePermissionBuilder Delete()
    {
        Assign<PageTemplateDeletePermission>();
        return this;
    }

    /// <summary>
    /// Permission to access the page templates module in the admin panel.
    /// </summary>
    public PageTemplatePermissionBuilder AdminModule()
    {
        Assign<PageTemplateAdminModulePermission>();
        return this;
    }
}
