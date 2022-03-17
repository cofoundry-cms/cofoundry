namespace Cofoundry.Web;

/// <summary>
/// Checks to see if the user has permissions to update a page/entity
/// and downgrades the edititor mode to draft view if the permissions are 
/// missing.
/// </summary>
public interface IValidateEditPermissionsRoutingStep : IPageActionRoutingStep
{
}
