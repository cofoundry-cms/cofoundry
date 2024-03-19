﻿using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

/// <summary>
/// The default implementation throws an exception because you cannot set
/// up the site without the admin plugin. The real implementation is
/// in Cofoundry.Web.Admin.SetupPageActionFactory.
/// </summary>
public class DefaultSetupPageActionFactory : ISetupPageActionFactory
{
    /// <inheritdoc/>
    public ActionResult GetSetupPageAction(Controller controller)
    {
        throw new Exception("Cofoundry has not been setup. Admin panel is not installed so installation must be done manually.");
    }
}
