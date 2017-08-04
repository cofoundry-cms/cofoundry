using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Data model representing a single image
    /// </summary>
    public class ImageDataModel : IPageBlockTypeDataModel
    {
        [Image]
        public int ImageId { get; set; }
        public string AltText { get; set; }
        public string LinkPath { get; set; }
        public string LinkTarget { get; set; }
    }
}