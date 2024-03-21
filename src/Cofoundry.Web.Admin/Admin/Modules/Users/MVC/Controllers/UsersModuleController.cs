using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class UsersModuleController : BaseAdminMvcController
{
    public UsersModuleController(
        IUserAreaDefinitionRepository userAreaDefinitionRepository
        )
    {
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
    }

    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

    public ActionResult Index()
    {
        var userArea = RouteData.DataTokens["UserArea"] as IUserAreaDefinition;
        if (userArea == null)
        {
            throw new Exception($"RouteData.DataTokens[\"UserArea\"] is null or not castable to IUserAreaDefinition.");
        }

        var userAreaOptions = _userAreaDefinitionRepository.GetOptionsByCode(userArea.UserAreaCode);

        var options = new UsersModuleOptions()
        {
            UserAreaCode = userArea.UserAreaCode,
            Name = userArea.Name,
            AllowPasswordSignIn = userArea.AllowPasswordSignIn,
            UseEmailAsUsername = userArea.UseEmailAsUsername,
            ShowDisplayName = !userAreaOptions.Username.UseAsDisplayName
        };

        var viewPath = ViewPathFormatter.View("Users", nameof(Index));
        return View(viewPath, options);
    }
}
