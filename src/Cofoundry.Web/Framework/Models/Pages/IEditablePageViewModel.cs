using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public interface IEditablePageViewModel
    {
        PageRenderDetails Page { get; set; }
        bool IsPageEditMode { get; set; }
    }
}