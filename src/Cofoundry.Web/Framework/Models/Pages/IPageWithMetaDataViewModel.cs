using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public interface IPageWithMetaDataViewModel
    {
        string PageTitle { get; set; }
        string MetaDescription { get; set; }
    }
}