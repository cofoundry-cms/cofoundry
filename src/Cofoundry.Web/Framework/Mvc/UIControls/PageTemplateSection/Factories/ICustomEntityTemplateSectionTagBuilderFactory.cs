using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public interface ICustomEntityTemplateSectionTagBuilderFactory
    {
        ICustomEntityTemplateSectionTagBuilder<TModel> Create<TModel>(
            HtmlHelper htmlHelper,
            CustomEntityDetailsPageViewModel<TModel> customEntityViewModel,
            string sectionName
            )
            where TModel : ICustomEntityDetailsDisplayViewModel
            ;
    }
}
