This section covers the following data annotations:

- [`[Color]`](#color)
- [`[Placeholder]`](#placeholder)
- [`[ReadOnly]`](#readonly)

Each of these are explained below:

## [Color]

The `[Color]` data annotation can be used to decorate a `string` property and provide a UI hint to the admin interface to display a color picker field. 

The editor will validate a hexadecimal color value e.g. "#EFEFEF" or "#fff".

#### Example

```csharp
using Cofoundry.Domain;

public class ExampleDataModel : ICustomEntityDataModel
{
    [Color]
    public string ExampleColor { get; set; }
}
```

Output:

![Color field example](images/color-field-example.png)

## [Placeholder]

The `[Placeholder]` data annotation can be used to provide a UI hint to the admin interface to add a [placeholder attribute](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/Input#htmlattrdefplaceholder) in an html input field.

#### Example

```csharp
using Cofoundry.Domain;

public class ExampleDataModel : ICustomEntityDataModel
{
    [Placeholder("Type here")]
    public string? ExamplePlaceholder { get; set; }
}
```

Output:

![Placeholder example showing placeholder text in an input field](images/placeholder-data-annotation-example.png)

## [ReadOnly]

The `[ReadOnly]` data annotation indicates that a property should not be editable in the admin UI. 

Other annotations can still be used to indicate the type of editor to use to display the read-only value e.g. `[Html]` would render the value as Html, and `[Date]` would format the value as a date without a time.

This attribute only affects the display of the property in the admin panel, and values can still be updated programmatically.
    
#### Example

```csharp
using Cofoundry.Domain;

public class ExampleDataModel : ICustomEntityDataModel
{
    [Date]
    public DateTime EditableDate { get; set; }

    [ReadOnly]
    [Date]
    public DateTime ReadOnlyDate { get; set; }
}
```

Output:

![Example showing how the ReadOnly attribute readers int he admin UI, showing an editable date and a readonly date field](images/readonly-dates-example.png)