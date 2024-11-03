Cofoundry allows you to define custom data types which can be fully managed in the admin panel with minimal configuration. We call these *Custom Entities* and they are useful for quickly defining custom content for your site that isn't easily represented by a page. The classic example is a blog post, but this could easily be a product or other piece of data used in various places around your site. Custom entities have the following features:

- Configuration is done in code via a simple definition class
- Any custom data you want to add to the custom entity is defined in code in a simple object, which is persisted as JSON in the database.
- Data is versioned using the same draft/publish system as Pages
- A section in the admin panel is created for each custom entity which allows you to search, view and manage them.
- There are built in editors for all the core data types (strings, ints, images, image collections, custom entity collections). Editor behavior is controlled via data attributes, and you can make you own if you need to support a custom editor specific to your application.
- Like pages, custom entities can be partitioned by locale and the title/url slug can be forced to be unique as a configuration option.
- Special page types can be set up to dynamically generate a page-per-custom entity (e.g. a blog page). This allows custom entity data to be edited in the same visual way that you would edit the data regions in a page.
- Lifecycle events are integrated with the `IMessageAggregator` framework so they can be subscribed to (e.g. Add/Update/Delete/Publish)
- [Permission sets](/framework/roles-and-permissions) are automatically generated for each custom entity which you can manage through the *Roles* interface.
- Automatic handling of relations between entities, i.e. when an entity has a required relationship with another entity, it will not be able to be deleted. This works as long as the relations are specified using [meta data attributes](/content-management/data-model-annotations).

### When not to use Custom Entities

Custom entities are great for many typical scenarios and have a great deal of flexibility built in, however they aren't designed to be used in all scenarios. They are not appropriate for:

- Complex hierarchical data models. Due to their complexity these can sometimes be more simply expressed through a custom implementation. You can still leverage the admin UI tools to build the editor interfaces.
- Per-user data, e.g. user profile data
- Log style data, e.g. contact form responses

## Getting Started

### Create a Data Model

The data model class will allow us to define any additional properties we want to include in our custom entity beyond the standard properties (*CustomEntityId*, *Title*, *UrlSlug*)

1. Create a class that implements `ICustomEntityDataModel`. You should use the naming convention *{MyEntity}DataModel*.
2. Add any properties you need to store.
3. Add validation attributes
4. Add UI MetaData attributes, see a list of options [here](/content-management/data-model-annotations).

Example:

```csharp
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

public class CatDataModel : ICustomEntityDataModel
{
    [Display(Description = "A short description to describe the cat")]
    public string? Description { get; set; }

    [Display(Name = "Features", Description = "Extra properties that help categorize this cat")]
    [CustomEntityCollection(FeatureCustomEntityDefinition.DefinitionCode)]
    public int[] FeatureIds { get; set; } = [];

    [Display(Name = "Images", Description = "The top image will be the main image that displays in the grid")]
    [ImageCollection]
    public int[] ImageAssetIds { get; set; } = [];
}
```

### Create a Definition

1. Create a class that implements `ICustomEntityDefinition<TDataModel>` where TDataModel is the type of your DataModel class
2. Implement the interface, noting that the `CustomEntityDefinitionCode` needs to be unique and the convention is to use uppercase. 
3. There are some other interfaces you can implement to give additional behaviors, these are documented below

Example:

```csharp
public class CatCustomEntityDefinition : ICustomEntityDefinition<CatDataModel>
{
    /// <summary>
    /// This constant is a convention that allows us to reference this definition code 
    /// in other parts of the application (e.g. querying)
    /// </summary>
    public const string DefinitionCode = "EXACAT";

    /// <summary>
    /// Unique 6 letter code representing the module (the convention is to use uppercase)
    /// </summary>
   public string CustomEntityDefinitionCode => DefinitionCode;

    /// <summary>
    /// Singlar name of the entity
    /// </summary>
    public string Name => "Cat";

    /// <summary>
    /// Plural name of the entity
    /// </summary>
    public string NamePlural => "Cats";

    /// <summary>
    /// A short description that shows up as a tooltip for the admin 
    /// panel.
    /// </summary>
    public string Description => "Each cat can be rated by the public.";

    /// <summary>
    /// Indicates whether the UrlSlug property should be treated
    /// as a unique property and be validated as such.
    /// </summary>
    public bool ForceUrlSlugUniqueness => false;

    /// <summary>
    /// Indicates whether the url slug should be autogenerated. If this
    /// is selected then the user will not be shown the UrlSlug property
    /// and it will be auto-generated based on the title.
    /// </summary>
    public bool AutoGenerateUrlSlug => true;

    /// <summary>
    /// Indicates whether this custom entity should always be published when 
    /// saved, provided the user has permissions to do so. Useful if this isn't
    /// the sort of entity that needs a draft state workflow
    /// </summary>
    public bool AutoPublish => false;

    /// <summary>
    /// Indicates whether the entities are partitioned by locale
    /// </summary>
    public bool HasLocale => false;
}
```

### Run the Project

When you run the project the definition will automatically be added to the database and you'll be able to edit the custom entities in the admin interface.

Currently you'll need to manually add permissions to any user roles (including the anonymous role) that should have permission to access the entity (Super Administrators have access to everything by default).

## Additional Definition Options

### IOrderableCustomEntityDefinition

Implement this interface to define a custom entity type that can have it's list ordering set.

- **Ordering:** Indicates the type of ordering permitted on this custom entity type.

    - *Full:* Each custom entity will have an ordering set. This is only intended for smaller lists of entities and will not work for collections with more than 60 elements.
    - *Partial:* Partial ordering where an ordering is specified for a subset of entities and the rest take a natural ordering.

### ISortedCustomEntityDefinition

Implement this interface on your custom entity definition class to define a default sort type and sort direction. This apply to all queries where no sort type is specified and so is most useful for customizing the default ordering or items in the list view in the admin panel.

- **DefaultSortType:** The sorting to apply by default when querying collections of custom entities of this type. A query can specify a sort type to override this value.
- **DefaultSortDirection:** The default sort direction to use when ordering with the default sort type.

### ICustomizedTermCustomEntityDefinition

Implement this interface to define custom terminology to use in the UI for a custom entity. Otherwise default terms will be used.

- **CustomTerms:** A dictionary of any custom terminology to use when displaying the custom entity, e.g. you could replace the terms for "Title" or "Url Slug". You can use the values in `CustomizableCustomEntityTermKeys` constants class for the keys.

## Additional Data Model Features

### Default Property Values

Any default values you set in a data model will be used to populate a new instance of your model in the admin panel.

Example:

```csharp
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

public class DogDataModel : ICustomEntityDataModel
{
    public DogDataModel()
    {
        DateOfBirth = DateTime.UtcNow;
    }

    [Date]
    public DateTime DateOfBirth { get; set; }

    [Color]
    public string FurColor { get; set; } = "#663399";
}
```
## Accessing & Displaying Custom Entity Data

Custom entity data can be used in a number of different ways:

- **Repository Access:** `IContentRepository` gives you a wide range of data access APIs that you can use in any way you choose. See [Accessing Data Programmatically](accessing-data programmatically) for more details.
- **Page Blocks:** When creating data models for page block types, you can use the `[CustomEntity]`, `[CustomEntityCollection]` or `[CustomEntityMultiTypeCollection]` data annotations to create data fields that link to custom entities.
- **Custom Entity Pages:** To dynamically create a details page for every custom entity (e.g. a blog article or product details page) follow the guidance [here](custom-entity-pages).

## What's Happening Under The Hood

Whilst the core custom entity properties, definition and version data is stored as *structured data* (relational SQL tables), the data models are stored as *unstructured data* (serialized JSON). This helps us strike a balance between speed, integrity and flexibility. For custom entities we sacrifice a little of this speed and data integrity to provide super flexibility, but we do have some measures in place to mitigate the downsides such as caching, batch lookups and dependency logging.