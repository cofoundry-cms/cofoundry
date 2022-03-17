using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

public interface IControllerResponseHelper
{
    Task ExecuteIfValidAsync<TCommand>(Controller controller, TCommand command) where TCommand : ICommand;
}
