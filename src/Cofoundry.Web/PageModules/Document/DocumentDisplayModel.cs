using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class DocumentDisplayModel : IPageModuleDisplayModel
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}