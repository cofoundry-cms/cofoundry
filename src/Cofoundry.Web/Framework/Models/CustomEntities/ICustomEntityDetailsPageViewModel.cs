using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public interface ICustomEntityDetailsPageViewModel<TModel> 
        : IPageWithMetaDataViewModel, IEditablePageViewModel, IPageRoutableViewModel 
        where TModel : ICustomEntityDetailsDisplayViewModel
    {
        CustomEntityRenderDetailsViewModel<TModel> CustomEntity { get; set; }
    }
}