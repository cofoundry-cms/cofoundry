using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class ErrorPageViewModel : IErrorPageViewModel
    {
        public int StatusCode { get; set; }

        public string StatusCodeDescription { get; set; }

        public string PageTitle { get; set; }

        public string MetaDescription { get; set; }
        
        public string PathBase { get; set; }

        public string Path { get; set; }

        public string QueryString { get; set; }
    }
}
