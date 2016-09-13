using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public interface IPageTemplateSectionTagBuilderFactory
    {
        IPageTemplateSectionTagBuilder Create(
            HtmlHelper htmlHelper,
            IEditablePageViewModel pageViewModel,
            string sectionName
            );
    }
}
