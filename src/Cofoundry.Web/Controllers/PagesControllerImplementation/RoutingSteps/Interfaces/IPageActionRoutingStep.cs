using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

public interface IPageActionRoutingStep
{
    Task ExecuteAsync(Controller controller, PageActionRoutingState state);
}
