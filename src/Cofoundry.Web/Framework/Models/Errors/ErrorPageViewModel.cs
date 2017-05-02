using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class ErrorPageViewModel : IPageWithMetaDataViewModel
    {
        public ErrorPageViewModel(Exception exception, string controllerName, string actionName)
        {
            Exception = exception;
            ControllerName = controllerName;
            ActionName = actionName;
        }

        public Exception Exception { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string PageTitle { get; set; }
        public string MetaDescription { get; set; }
    }
}
