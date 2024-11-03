
Roles and permissions can be managed either through the admin panel or defined in code.

## Managing Roles in the Admin Panel

Roles can be managed in the admin panel in the *Roles* section. Here you can add new roles that are managed within the interface, and you can also update the permissions of roles that have been defined in code. 

There are two built-in roles that cannot be removed:

- **Anonymous:** The role assigned to users who are not signed in i.e. browsing your site without signing in. The permissions for this role can be managed in the admin panel.
- **Super Administrator:** This is the default administrator role which is assigned all permissions. This role is not editable.

## Managing Roles in Code

Managing a role in code has two advantages: the first is that it is automatically added to the system on startup, the second is that code-based roles use a unique string code that makes it easier and more reliable to query the role programmatically.

### Defining a role

To define a role in code, create a class that implements `IRoleDefinition`:

```csharp
using Cofoundry.Domain;

public class MarketingRole : IRoleDefinition
{
    /// <summary>
    /// By convention we add a constant for the role code
    /// to make it easier to reference.
    /// </summary>
    public const string Code = "MKT";

    /// <summary>
    /// The role title is used to identify the role and select it in the admin 
    /// UI and therefore must be unique. Max 50 characters.
    /// </summary>
    public string Title =>  "Marketing";

    /// <summary>
    /// The role code is a unique three letter code that can be used to reference the
    /// role programmatically. The code must be unique and convention is to use upper 
    /// case, although code matching is case insensitive.
    /// </summary>
    public string RoleCode => Code;

    /// <summary>
    /// A role must be assigned to a user area, in this case we are adding it to
    /// the Cofoundry user area so they will have access to the Cofoundry admin panel.
    /// </summary>
    public string UserAreaCode => CofoundryAdminUserArea.Code;
    
    /// <summary>
    /// This method determines which permissions the role is granted when it is first created. 
    /// To help do this you are provided with a builder that contains all permissions in the 
    /// system which you can use to either include or exclude permissions based on rules.
    /// </summary>
    public void ConfigurePermissions(IPermissionSetBuilder builder)
    {
        builder
            .IncludeAll()
            .ExcludeAllDelete()
            .ExcludeUserInAllUserAreas()
            .ExcludeRole()
            .IncludeRole(rolePermissions => rolePermissions.Read())
            .ExcludeSettings(settingsPermissions => settingsPermissions.UpdateGeneral());
    }
}
```

### Configuring permissions in code

In the example above, the `ConfigurePermissions(IPermissionSetBuilder builder)` method is used to define the permissions for the role. This configuration is applied when the role is first created, and will only run again to apply any new permissions that are added to the system during an update. This means that the permissions assigned to a code-based role can be updated via the admin panel after it has been created.

Leaving this method empty will prevent any permissions being assigned at startup, but it's good practice to add in some default permissions so they can get automatically applied across environments.

The "Marketing" role example above uses an opt-out configuration style, starting with a baseline of all permissions and removing those which are not needed. This is useful if there are only a few particular permissions you want to exclude, but more commonly you might want to start from a minimal baseline, in which case it's useful to copy from the anonymous role:

```csharp
public void ConfigurePermissions(IPermissionSetBuilder builder)
{
    builder
        .ApplyAnonymousRoleConfiguration()
        .IncludeCurrentUser(c => c.Update());
}
```

You can also copy permission configuration from any other role using `builder.ApplyRoleConfiguration<ExampleRole>()`.

### Customizing the Anonymous Role

By default the anonymous role only includes read permissions to entities (excluding users). If you want to customize this you can create a class that inherits from `IAnonymousRolePermissionConfiguration`:

```csharp
using Cofoundry.Domain;

public class ExampleAnonymousRoleInitializer : IAnonymousRolePermissionConfiguration
{
    public void ConfigurePermissions(IPermissionSetBuilder builder)
    {
        builder
            .IncludeAnonymousRoleDefaults()
            .Include<ExamplePermission>();
    }
}
```



