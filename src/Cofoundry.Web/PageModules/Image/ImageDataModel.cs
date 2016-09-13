using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class ImageDataModel : IPageModuleDataModel
    {
        [Image]
        public int ImageId { get; set; }
        public string AltText { get; set; }
        public string LinkPath { get; set; }
        public string LinkTarget { get; set; }
    }
}