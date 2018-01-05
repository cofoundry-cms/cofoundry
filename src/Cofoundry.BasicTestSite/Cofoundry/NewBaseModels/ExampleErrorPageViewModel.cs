using Cofoundry.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.BasicTestSite
{
    public class ExampleErrorPageViewModel : NotFoundPageViewModel
    {
        public string TestMessage { get; set; }
    }
}