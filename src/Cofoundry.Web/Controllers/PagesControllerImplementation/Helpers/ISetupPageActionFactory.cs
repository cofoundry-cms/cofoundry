using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

/// <summary>
/// Factory to detach the implementation of the site setup page. The main implementation 
/// of this is handled in Cofoundry.Web.Admin
/// </summary>
public interface ISetupPageActionFactory
{
    /// <summary>
    /// Gets an action result to display when setup is required
    /// </summary>
    ActionResult GetSetupPageAction(Controller controller);
}
