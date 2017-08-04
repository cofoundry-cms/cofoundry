using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class ImageDisplayModel : IPageBlockTypeDisplayModel
    {
        public string Source { get; set; }
        public string AltText { get; set; }
        public string LinkPath { get; set; }
        public string LinkTarget { get; set; }
    }
}