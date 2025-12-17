This plugin adds a `[YouTube]` data model attribute that can be used to markup a property of type `YouTubeVideo`. This will show as a YouTube Video picker in the admin UI.


## Installation

Install the [Cofoundry.Plugins.YouTube](https://www.nuget.org/packages/Cofoundry.Plugins.YouTube/) package via Nuget, e.g. via the CLI:

```bash
dotnet add package Cofoundry.Plugins.YouTube
```

## Configuration

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

## Usage

Add the `[YouTube]` data annotation to your data model to make the property editable with the YouTube Video picker in the admin UI:

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

## Sample Project

A [YouTubeSample](https://github.com/cofoundry-cms/cofoundry/tree/main/samples/src/Plugins/YouTubeSample) project can be found in the Cofoundry repository which contains a basic *YouTubeVideo* page block type.
