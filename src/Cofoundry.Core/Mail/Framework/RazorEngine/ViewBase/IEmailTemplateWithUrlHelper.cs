using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Used to mark up a RazorEngine base class with a url helper (e.g. EmailTemplateBase) 
    /// to allow the url helper to be injected.
    /// </summary>
    public interface IEmailTemplateWithUrlHelper
    {
        RazorEngineUrlHelper Url { get; set; }
    }
}
