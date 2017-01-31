using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class NotFoundPageViewModel : IPageWithMetaDataViewModel
    {
        public string PageTitle { get; set; }
        
        public string MetaDescription { get; set; }
    }
}