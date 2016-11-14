using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the page module types registered in the database using the
    /// IPageModuleDataModel types registered in the DI injector. This is typically
    /// run during the auto-update process when the application starst up.
    /// </summary>
    public class RegisterPageModuleTypesCommand : ICommand
    {
    }
}
