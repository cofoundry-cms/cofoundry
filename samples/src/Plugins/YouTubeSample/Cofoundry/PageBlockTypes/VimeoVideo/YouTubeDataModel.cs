using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;
using Cofoundry.Plugins.YouTube.Domain;

namespace YouTubeSample;

public class YouTubeVideoDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [Required]
    [YouTube]
    public YouTubeVideo? Video { get; set; }
}
