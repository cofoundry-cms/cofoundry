using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public class ErrorPageViewModel : HandleErrorInfo, IPageWithMetaDataViewModel
    {
        public ErrorPageViewModel(Exception exception, string controllerName, string actionName)
            : base(exception, controllerName, actionName)
        {

        }

        public string PageTitle { get; set; }
        public PageMetaData MetaData { get; set; }
    }
}
