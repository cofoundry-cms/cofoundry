Permissions can be used to control access to specific features across multiple roles. Cofoundry defines permissions for all it's entities including custom entities that have been dynamically generated from your code definitions, so it's not often you'll need to create your own permissions unless you are developing bespoke features that you need to control access to.

Permissions are defined in code and come in two flavors:

- **IPermission:** A basic permission for an action not associated with a specific entity e.g. 'View Error Log'
- **IEntityPermission:** A permission that relates to an entity type, e.g. 'Add Page' or 'Delete Image Asset'

Typically when developing a new area of functionality, it will be based around a specific entity type, so you'll want to opt for `IEntityPermission` in this case. Cofoundry defines it's permissions using the same system, so check out the [source code](https://github.com/cofoundry-cms/cofoundry/tree/master/src/Cofoundry.Domain/Domain/) for examples. 

Using [Pages](https://github.com/cofoundry-cms/cofoundry/tree/master/src/Cofoundry.Domain/Domain/Pages/Permissions) as an example, you can see each permission has its own C# class and the permissions all inherit from `IEntityPermission`. Each permission uses `PageEntityDefinition` for the `EntityDefinition` property, while for the `PermissionType` property some use the predefined types in the `CommonPermissionTypes` constants library (e.g. CRUD operations) and others use custom `PermissionType` objects (e.g. `PageUpdateUrlPermission`).

#### Example IPermission Implementation

```csharp
public class VipDealsPermission : IPermission
{
    public PermissionType PermissionType => new("VIPDEA", "VIP Deals", "Access to VIP deals");
}
```

#### Example IEntityPermission Implementation

```csharp
public class PageReadPermission : IEntityPermission
{
    public IEntityDefinition EntityDefinition => new PageEntityDefinition();
    
    public PermissionType PermissionType => CommonPermissionTypes.Read("Pages");
}
```

