using Cofoundry.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.BasicTestSite
{
    public class ExampleCustomEntityPageViewModel<TModel> : CustomEntityPageViewModel<TModel>
        where TModel : ICustomEntityPageDisplayModel
    {
        public string TestMessage { get; set; }
    }
}