using Cofoundry.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.BasicTestSite
{
    public class ExampleCustomEntityPageViewModel<TModel> : CustomEntityDetailsPageViewModel<TModel>
        where TModel : ICustomEntityDetailsDisplayViewModel
    {
        public string TestMessage { get; set; }
    }
}