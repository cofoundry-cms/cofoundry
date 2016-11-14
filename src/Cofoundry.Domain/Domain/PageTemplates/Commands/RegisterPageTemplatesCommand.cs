using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the page templates registered in the database by scanning
    /// for template files via IPageTemplateViewFileLocator. This is typically
    /// run during the auto-update process when the application starts up.
    /// 
    /// Removed templates are only deleted from the system if they have no
    /// dependencies, otherwise they are marked as archived which allows for
    /// data migration and prevents unintended deletions of page content.
    /// </summary>
    public class RegisterPageTemplatesCommand : ICommand
    {
    }
}
