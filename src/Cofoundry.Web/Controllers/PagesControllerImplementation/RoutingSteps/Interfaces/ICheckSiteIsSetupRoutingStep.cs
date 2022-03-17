namespace Cofoundry.Web;

/// <summary>
/// Checks if Cofoundry has been set up yet and if it hasn't, redirects the request
/// to the setup screen.
/// </summary>
public interface ICheckSiteIsSetupRoutingStep : IPageActionRoutingStep
{
}
