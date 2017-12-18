using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public interface ICustomEntityPageViewModel<TDisplayModel>
        : IPageWithMetaDataViewModel, IEditablePageViewModel, IPageRoutableViewModel
        where TDisplayModel : ICustomEntityPageDisplayModel
    {
        CustomEntityRenderDetailsViewModel<TDisplayModel> CustomEntity { get; set; }
    }
}
