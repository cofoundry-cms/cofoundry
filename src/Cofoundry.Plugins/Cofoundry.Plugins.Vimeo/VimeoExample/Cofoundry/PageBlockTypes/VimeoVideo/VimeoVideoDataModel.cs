using Cofoundry.Domain;
using Cofoundry.Plugins.Vimeo.Domain;

namespace VimeoExample;

public class VimeoVideoDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [Vimeo]
    public VimeoVideo? Video { get; set; }
}
