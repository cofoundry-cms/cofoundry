using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public interface IPageViewModel : IPageWithMetaDataViewModel, IEditablePageViewModel, IPageRoutableViewModel
    {
    }
}