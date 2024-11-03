# Cofoundry.Plugins.Vimeo

[![Build status](https://ci.appveyor.com/api/projects/status/e6m2qhk4rth6t1o4?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-vimeo)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.Vimeo.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.Vimeo/)


This library is a plugin for [Cofoundry](https://www.cofoundry.org/). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

This plugin adds a single data attribute `[Vimeo]` that can be used to markup a property of type `VimeoVideo`. This will show as a Vimeo Video picker in the admin UI.

## Example

You can find a full example project named **VimeoExample** in the solution in this repository, which contains a *VimeoVideo* page block type. 

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




