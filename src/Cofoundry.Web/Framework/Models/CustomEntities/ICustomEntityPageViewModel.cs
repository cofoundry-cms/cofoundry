using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public interface ICustomEntityPageViewModel<TModel> 
        : IPageWithMetaDataViewModel, IEditablePageViewModel, IPageRoutableViewModel 
        where TModel : ICustomEntityPageDisplayModel
    {
        CustomEntityRenderDetailsViewModel<TModel> CustomEntity { get; set; }
    }
}