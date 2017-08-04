using Cofoundry.Domain;
using Cofoundry.Web;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    /// <summary>
    /// Each block type must have a display model that is marked with
    /// IPageModuleDisplayModel. This is the model that is rendered 
    /// in the view. In simple scenarios you can simply mark up the data 
    /// model as the display model, but here we want to transform the 
    /// HtmlText property so we need to define a separate model.
    /// 
    /// See https://github.com/cofoundry-cms/cofoundry/wiki/Page-Module-Types
    /// for more information
    /// </summary>
    public class ContentSectionDisplayModel : IPageBlockTypeDisplayModel, IPageBlockWithParentPageData
    {
        public string Title { get; set; }

        public IHtmlContent HtmlText { get; set; }

        public IEditablePageViewModel ParentPage { get; set; }
    }
}