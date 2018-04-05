using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class ImageAssetFileServiceNotImplementedException : Exception
    {
        const string HELP_LINK = "https://github.com/cofoundry-cms/cofoundry/wiki/Images#supporting-image-assets";
        const string MESSAGE = "No image file plugin installed. To use image assets you need to reference an image asset plugin. See " + HELP_LINK;

        public ImageAssetFileServiceNotImplementedException()
            : base(MESSAGE)
        {
            this.HelpLink = HELP_LINK;
        }
    }
}
