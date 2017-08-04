using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class TextListDisplayModel : IPageBlockTypeDisplayModel
    {
        public string Title { get; set; }
        public string[] TextListItems { get; set; }
        public bool IsNumbered { get; set; }
    }
}