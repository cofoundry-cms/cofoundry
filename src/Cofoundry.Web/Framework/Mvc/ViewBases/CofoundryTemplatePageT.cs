using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryTemplatePage<TModel> 
        : System.Web.Mvc.WebViewPage<TModel> where TModel : IEditablePageViewModel
    {
        public override void InitHelpers()
        {
            base.InitHelpers();
            Cofoundry = new CofoundryTemplatePageHelper<TModel>(Html, this.Model);
        }

        public CofoundryTemplatePageHelper<TModel> Cofoundry { get; private set; }
    }

}
