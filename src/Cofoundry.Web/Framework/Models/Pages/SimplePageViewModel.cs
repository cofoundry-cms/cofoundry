using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Simple implementation of IPageWithMetaDataViewModel for pages that don't need much else.
    /// </summary>
    public class SimplePageViewModel : IPageWithMetaDataViewModel
    {
        public string PageTitle { get; set; }
        
        public string MetaDescription { get; set; }
    }
}