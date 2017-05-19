using Cofoundry.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    public class ExamplePageViewModelFactory : IPageViewModelFactory
    {
        public ICustomEntityDetailsPageViewModel<TDisplayModel> CreateCustomEntityDetailsPageViewModel<TDisplayModel>() where TDisplayModel : ICustomEntityDetailsDisplayViewModel
        {
            return new ExampleCustomEntityPageViewModel<TDisplayModel>();
        }

        public IPageViewModel CreatePageViewModel()
        {
            return new ExamplePageViewModel();
        }

        public INotFoundPageViewModel CreateNotFoundPageViewModel()
        {
            return new ExampleNotFoundPageViewModel();
        }
    }
}