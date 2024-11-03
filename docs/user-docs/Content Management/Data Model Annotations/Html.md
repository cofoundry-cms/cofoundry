The `[Html]` data annotation can be used to decorate a string property and provides a UI hint to the admin interface to display an html editor field. Cofoundry uses the [TinyMCE](https://www.tinymce.com/) editor to edit html content.

```csharp
public class ExampleDataModel : ICustomEntityDataModel
{
    [Html]
    public string? Content { get; set; }
}
```

## Customizing the toolbar

The `HtmlToolbarPreset` enum defines a few preset toolbars that can be used to control the set of toolbars that are rendered with the html editor:

- **Headings:** A style selector for h1-h3 headings and normal text.
- **BasicFormatting:** Buttons for bold, italic, underline, links  and clear formatting.
- **AdvancedFormatting:** Buttons for alignment, blockquote & lists, strikethrough and superscript/subscript.
- **Media:** Buttons to insert pictures, video
- **Source:** Edit html source button.
- **Custom:** Indicates the position to insert a custom toolbar (see below)

By default the `Headings` and `BasicFormatting` toolbars will be displayed. You can change the default by specifying them in the attribute constructor:

```csharp
public class ExampleDataModel : ICustomEntityDataModel
{
    [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Media, HtmlToolbarPreset.Source)]
    public string? Content { get; set; }
}
```

### Adding a custom toolbar

You can use the `HtmlToolbarPreset.Custom` enum value to insert a custom toolbar, and use the `CustomToolbar` property to define the buttons. You can see a full list of buttons in the [TinyMCE toolbar documentation](https://www.tinymce.com/docs/advanced/editor-control-identifiers/#toolbarcontrols), and use the pipe separator to split toolbars. 

In addition to the buttons included with TinyMCE, there is also a button for inserting images from the Cofoundry image library which you can reference with the keyword `cfimage`:

```csharp
public class ExampleDataModel : ICustomEntityDataModel
{
    [Html(HtmlToolbarPreset.Custom, CustomToolbar = "undo redo | bold italic underline | link unlink | cfimage")]
    public string? Content { get; set; }
}
```

## Editor height/rows

You can set the editor height using the `Rows` property, the same way you can with the `[MultiLineText]` attribute. The default is 20.

```csharp
public class ExampleDataModel : ICustomEntityDataModel
{
    [Html(Rows=10)]
    public string? Content { get; set; }
}
```

## Customizing the TinyMCE Config

You can completely control the configuration of TinyMCE by defining your own configuration. There ware two properties you can use to do this:

#### ConfigSource

A type to use to determine any additional configuration options to apply to the html editor. This should be a class that inherits from `IHtmlEditorConfigSource`, which provides a .NET code generated set of options.

```csharp
public class ExampleHtmlEditorConfigSource : IHtmlEditorConfigSource
{
    private static readonly Dictionary<string, object> _options = new Dictionary<string, object>()
    {
        { "resize", false },
        { "browser_spellcheck", false }
    };

    public IDictionary<string, object> Create()
    {
        return _options;
    }
}

public class ExampleDataModel : ICustomEntityDataModel
{
    [Html(ConfigSource = typeof(ExampleHtmlEditorConfigSource))]
    public string Content { get; set; }
}
```

#### ConfigFilePath

You can also define a path to a JSON configuration file if you prefer to write your config in JSON:

**html-editor-config.json**

```js
{
  "resize": false,
  "browser_spellcheck": false,
  "toolbar": "undo redo | bold italic underline | link unlink | preview forecolor backcolor",
  "plugins": "link preview textcolor",
}
```

**ExampleDataModel.cs**

```csharp
public class ExampleDataModel : ICustomEntityDataModel
{
    [Html(ConfigFilePath = "/content/html-editor-config.json")]
    public string? Content { get; set; }
}
```

## Escaping and Sanitizing

When working with html you need to be aware of two things:

#### 1. Tiny MCE cleans HTML

TinyMCE will try and clean up any html and remove what it thinks are invalid or dangerous elements. If you have users pasting in code from other sources e.g. internet or document files, then this can be useful to keep the input clean, however this can be a problem if your inputting raw html snippets or JavaScript content.

To prevent TinyMCE from cleaning up code you can override the config to allow all elements:

```csharp
public class AllowAllElementsHtmlEditorConfigSource : IHtmlEditorConfigSource
{
    public IReadOnlyDictionary<string, object> Create()
    {
        // configure TinyMCE to permit all elements
        return new Dictionary<string, object>() 
        {
            { "valid_elements", "+*[*]" }
        };
    }
}

public class ExampleDataModel : ICustomEntityDataModel
{
    [Html(ConfigSource = typeof(AllowAllElementsHtmlEditorConfigSource))]
    public string? Content { get; set; }
}
```

#### 2. You should consider sanitizing when rendering HTML

Cofoundry includes a sanitizer than you can use to clean up HTML when rendering content and prevent [XSS attacks](https://www.owasp.org/index.php/Cross-site_Scripting_(XSS)). 

Take a look at the [sanitizer documentation](/framework/html-sanitizer) for more information.

## Custom html editor attributes

If you're using a specific configuration often, you may want to consider deriving a new data annotation from the base `HtmlAttribute`.

```csharp
[AttributeUsage(AttributeTargets.Property)]
public class HtmlWithCustomEditorAttribute : HtmlAttribute
{
    public HtmlWithCustomEditorAttribute()
        : base(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Media, HtmlToolbarPreset.Source)
    {
        ConfigFilePath = "/content/html-editor-config.json";
        Rows = 40;
    }
}

public class ExampleDataModel : ICustomEntityDataModel
{
    [HtmlWithCustomEditor]
    public string Content { get; set; }
}
```
