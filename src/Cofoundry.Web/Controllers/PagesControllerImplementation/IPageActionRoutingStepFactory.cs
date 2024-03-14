namespace Cofoundry.Web;

/// <summary>
/// A factory that creates a collection of ordered <see cref="IPageActionRoutingStep"/> to
/// be executed in order during Cofoundry's page controller action. This determines
/// the routing of dynamic page content in the site.
/// </summary>
public interface IPageActionRoutingStepFactory
{
    /// <summary>
    /// Creates a collection of ordered <see cref="IPageActionRoutingStep"/> to
    /// be executed in order during Cofoundry's page controller action. This determines
    /// the routing of dynamic page content in the site.
    /// </summary>
    IEnumerable<IPageActionRoutingStep> Create();
}
