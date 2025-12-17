This plugin adds a `[Vimeo]` data model attribute that can be used to markup a property of type `VimeoVideo`. This will show as a Vimeo Video picker in the admin UI.

## Installation

Install the [Cofoundry.Plugins.Vimeo](https://www.nuget.org/packages/Cofoundry.Plugins.Vimeo/) package via Nuget, e.g. via the CLI:

```bash
dotnet add package Cofoundry.Plugins.Vimeo
```

## Usage

Add the `[Vimeo]` data annotation to your data model to make the property editable with the Vimeo Video picker in the admin UI:

**Example Data Model**

```csharp
using Cofoundry.Domain;
using Cofoundry.Plugins.Vimeo.Domain;
using System.ComponentModel.DataAnnotations;

public class VimeoVideoDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [Required]
    [Vimeo]
    public VimeoVideo Video { get; set; }
}

```

## Sample Project 

A [VimeoSample](https://github.com/cofoundry-cms/cofoundry/tree/main/samples/src/Plugins/VimeoSample) project can be found in the Cofoundry repository which contains a basic *VimeoVideo* page block type. 
