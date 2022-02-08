namespace Cofoundry.Web
{
    /// <summary>
    /// Validate that the currently logged in user can access the route. If
    /// the user fails any access rules checks, then the action associated 
    /// with the rule is carried out e.g. redirect to sign in, 404, throw error.  
    /// </summary>
    public interface IValidateAccessRulesRoutingStep : IPageActionRoutingStep
    {
    }
}