There are a few places in Cofoundry that auto-generate data entry forms from annotated POCO data models, such as [Page Block Types](/content-management/page-block-types) and [Custom Entities](/content-management/custom-entities).  

## Primitive Types

Primitive types such as `string` and `int` will be displayed with a basic editor that corresponds with their type. The basic editor can be overridden by adding a data annotation, for example a `string` property automatically generates a single line text input, but you can use the `[MultiLineText]` attribute to turn this into a textarea.

[Read more](primitives)

## .NET DataAnnotations

Models are always validated on the server and so the built-in .NET data annotations will work as expected. 

The following .NET DataAnnotations will also provide client-side enhancements in the admin UI:

- `[Compare]`: Compares two fields to ensure their values match.
- `[Display]`: The `Name` and `Description` properties are used when displaying a field in the admin panel.
- `[EmailAddress]`: Displays an email input field. 
- `[MinLength]`, `[MaxLength]`, `[StringLength]`: On string fields, this sets the valid length of the value.
- `[Range]`: Can be used on string fields or fields that support the min/max attributes, such as numbers to enforce min/max values.
- `[Required]`: Validates that a field has a value.
- `[Url]`: Displays a url input field.

## Built-in Cofoundry Attributes

Cofoundry has a range of built in data annotations that either enhance existing types or compliment Cofoundry entities like images and documents. 

#### UI/Validation Attributes

- **[[CheckboxList]](selection-lists#checkboxlist):** Use this to decorate a collection property to indicate it should be rendered as a list of checkbox inputs in the admin UI. Specify an option source to describe the items to display in the list.
- **[[Color]](miscellaneous#color):** Use this to decorate a string property and provide a UI hint to the admin interface to display an html editor field. Toolbar options can be specified in the constructor and the CustomToolbar property can be used to show a completely custom toolbar.
- **[[CustomEntity]](Custom-Entities):** Use this to decorate an integer property and indicate that it should be the id for a custom entity of a specific type.
- **[[CustomEntityCollection]](Custom-Entities#customentitycollection):** Allows a user to pick multiple custom entities of a specific type, which can optionally have 'drag and drop' ordering. Designed to be applied to an `ICollection<int>` property.
- **[[CustomEntityMultiTypeCollection]](Custom-Entities#customentitymultitypecollection):** Use this to decorate a collection of `CustomEntityIdentity` objects, indicating the property represents a set of custom entities of mixed types. Optional parameters indicate whether the collection is sortable.
- **[[DateAndTime]](dates-and-times#dateandtime):** Use this to decorate a DateTime, DateTimeOffset or string property and provide a UI hint to the admin interface to display a timezone-insensitive date and time picker field.
- **[[DateAndTimeLocal]](dates-and-times#dateandtimelocal):** Use this to decorate a DateTime, DatimeOffset or string property and provide a UI hint to the admin interface to display a timezone-sensitive date and time picker field.
- **[[Date]](dates-and-times#date):** Use this to decorate a DateTime, DateTimeOffset or string property and provide a UI hint to the admin interface to display a timezone-insensitive date picker field.
- **[[DateLocal]](dates-and-times#datelocal):** Use this to decorate a DateTime, DatimeOffset or string property and provide a UI hint to the admin interface to display a timezone-sensitive date picker field.
- **[[Document]](/content-management/data-model-annotations/Documents):** Use with an (optionally nullable) integer to indicate this is for the id of a document asset.
- **[[DocumentCollection]](Documents#documentcollection):** This data annotation can be used to decorate a collection of integers, indicating the property represents a set of document asset ids. The editor allows for sorting and you can set filters for restricting file types.
- **[[Html]](Html):** Use this to decorate a string property and provide a UI hint to the admin interface to display an html editor field. Toolbar options can be specified in the constructor and the CustomToolbar property can be used to show a completely custom toolbar.
- **[[Image]](Images):** Use with an (optionally nullable) integer to indicate this is for the id of an ImageAsset. A non-null integer indicates this is a required field. Optional parameters allow the search filter to be restricted e.g. width/height etc
- **[[ImageCollection]](Images#imagecollection):** Use this to decorate an integer array of ImageAssetIds and indicate that it should be a collection of image assets. The editor allows for sorting of linked assets and you can set filters for restricting image sizes.
- **[[MultiLineText]](primitives#multilinetext):** Use this to decorate a string property and provide a UI hint to the admin interface to display a text area field
- **[[NestedDataModelCollection]](nested-data-models):** Use this to decorate a collection of `INestedDataModel` objects, allowing them to be edited in the admin UI.
- **[[Number]](primitives#number):** Use this to decorate a numeric property and provide a UI hint to the admin interface to display an html5 number field. The step property can be used to specify the precision of the number e.g. 2 decimal places
- **[[Placeholder]](miscellaneous#placeholder):** Use this to provide a UI hint to the admin interface to add a placeholder attribute to an html input field.
- **[[PreviewTitle]](display-preview):** Indicates the property of a model that can be used as a title, name or short textual identifier. Typically this is used in a grid of items to identify the row.
- **[[PreviewImage]](display-preview):** Indicates the property of a model that can be used as the main image when displaying the model. Typically this is used in a grid of items to show an image representation of the row.
- **[[PreviewDescription]](display-preview):** Indicates the property of a model that can be used as a description field. Typically this is used in a grid of items to describe the item.
- **[[RadioListList]](selection-lists#radiolist):** Use this to decorate a collection property to indicate it should be rendered as a radio input list in the admin UI. Specify an option source to describe the items to display in the list.
- **[[ReadOnly]](miscellaneous#readonly):** This attribute can be used to indicate a property should not be editable in the admin UI. 
- **[[SelectListList]](selection-lists#selectlist):** Use this to decorate a collection property to indicate it should be rendered as a select list (drop down list) in the admin UI. Specify an option source to describe the items to display in the list.
- **[[Time]](dates-and-times#time):** Use this to decorate a TimeSpan or string property and provide a UI hint to the admin interface to display a time picker field.

#### Special Behavior Attributes

- **`[EntityDependency]`:** This can be used to decorate an integer id property that links to another entity. The entity must have a definition that implements `IDependableEntityDefinition`. Defining relations allow the system to detect and prevent entities used in required fields from being removed.
- **`[EntityDependencyCollection]`:** This can be used to decorate an integer id array property that links to a set of entities. The entity must have a definition that implements `IDependableEntityDefinition`. Defining relations allow the system to detect and prevent entities used in required fields from being removed.
- **`[CustomEntityRouteData]`:** Use this to mark up a property in a custom entity data model, this property will be extracted and added to the cached `CustomEntityRoute` object and therefore make the property available for routing operations without having to re-query the db. E.g. for a blog post custom entity you could mark up a category Id and then use this in an `ICustomEntityRoutingRule` to create a /category/blog-post URL route

## Creating Your Own Attributes

You can create your own attributes, but to get UI integration you'll need to to be familiar with Angular.js. We're not quite ready to document the Angular UI process just yet, but if you're interested you can see some examples of UI components on [github](https://github.com/cofoundry-cms/cofoundry/tree/master/src/Cofoundry.Web.Admin/Admin/Modules/Shared/Js/UIComponents)

When creating your annotations you should implement `IMetadataAttribute` and add any additional data you want to send to the angular client to the `AdditionalValues` collection. These will be rendered into your control with the 'cms-' prefix. 

The `TemplateHint` property can be used to customize the tag name that is output.

You can also add additional metadata to an existing attribute by creating an attribute class that implements `IModelMetaDataDecorator` 
