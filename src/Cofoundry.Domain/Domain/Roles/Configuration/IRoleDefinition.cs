namespace Cofoundry.Domain;

/// <summary>
/// As well as being able to create roles in the UI, roles can also
/// be defined in code by implementing <see cref="IRoleDefinition"/>. To defined the
/// permissions associated with the role implement a class that inherits 
/// from <see cref="IRoleInitializer"/>
/// </summary>
public interface IRoleDefinition
{
    /// <summary>
    /// The role title is used to identify the role and select it in the admin 
    /// UI and therefore must be unique. Max 50 characters.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// The role code is a unique three letter code that can be used to reference the 
    /// role programatically. The code must be unique and convention is to use upper 
    /// case, although code matching is case insensitive.
    /// </summary>
    string RoleCode { get; }

    /// <summary>
    /// A role must be assigned to a user area e.g. CofoundryAdminUserArea.
    /// </summary>
    string UserAreaCode { get; }

    /// <summary>
    /// Configures the permissions that that should be added to the role when
    /// it is first created. This is also invoked when new permissions are added
    /// to the system to determine whether it should be automatically added to the 
    /// role, based on the rules defined in the <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">
    /// Use the builder to configure the rules that determine whether a permission
    /// should be added to the system.
    /// </param>
    void ConfigurePermissions(IPermissionSetBuilder builder);
}
