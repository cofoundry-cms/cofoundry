using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class InvalidImageResizeSettingsException : Exception
    {
        public InvalidImageResizeSettingsException(string message)
            : base(message)
        {
        }

        public InvalidImageResizeSettingsException(string message, IImageResizeSettings imageResizeSettings)
            : base(message)
        {
            ImageResizeSettings = imageResizeSettings;
        }

        public IImageResizeSettings ImageResizeSettings { get; set; }
    }
}
