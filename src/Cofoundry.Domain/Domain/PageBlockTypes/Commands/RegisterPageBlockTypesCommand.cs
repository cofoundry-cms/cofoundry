using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the page block types registered in the database using the
    /// IPageBlockDataModel types registered in the DI injector. This is typically
    /// run during the auto-update process when the application starts up.
    /// </summary>
    public class RegisterPageBlockTypesCommand : ICommand
    {
    }
}
