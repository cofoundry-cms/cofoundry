using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

public interface IEmptyActionContextFactory
{
    ActionContext Create();
}
