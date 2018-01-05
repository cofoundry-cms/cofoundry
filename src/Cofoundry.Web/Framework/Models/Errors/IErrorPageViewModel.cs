using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public interface IErrorPageViewModel : IPageWithMetaDataViewModel
    {
        int StatusCode { get; set; }

        string StatusCodeDescription { get; set; }

        string PathBase { get; set; }

        string Path { get; set; }

        string QueryString { get; set; }
    }
}