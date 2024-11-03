# Cofoundry.Plugins.YouTube

[![Build status](https://ci.appveyor.com/api/projects/status/e6m2qhk4rth6t1o4?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-youtube)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.YouTube.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.YouTube/)


This library is a plugin for [Cofoundry](https://www.cofoundry.org/). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

This plugin adds a single data attribute `[YouTube]` that can be used to markup a property of type `YouTubeVideo`. This will show as a YouTube Video picker in the admin UI.

## API Data

The YouTube api requires [an api key](https://developers.google.com/youtube/v3/getting-started), so by default only the `YouTubeVideo.Id` property is returned in the data model. This might be sufficient for most purposes (e.g. using oEmbed), but if you want to pull in more data such as the video title, description or thumbnail image then you'll need to add an api key to your configuration settings:

```js
{
  "Cofoundry": {
    "Plugins": {
      "YouTube:ApiKey": "TODO:AddApiKey"
    }
  }
}
```
Once you've added this api key, the UI editor will automatically start pulling in extra data into your `YouTubeVideo` model.

## Example

You can find a full example project named **YouTubeExample** in the solution in this repository, which contains a *YouTubeVideo* page block type. 

**Example Data Model**

```csharp
using Cofoundry.Domain;
using Cofoundry.Plugins.YouTube.Domain;
using System.ComponentModel.DataAnnotations;

public class YouTubeVideoDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [Required]
    [YouTube]
    public YouTubeVideo Video { get; set; }
}

```




