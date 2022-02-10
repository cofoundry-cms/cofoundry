using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    [Route("members")]
    public class MemberController : Controller
    {
        private readonly IAdvancedContentRepository _contentRepository;
        private readonly IAuthorizedTaskTokenUrlHelper _authorizedTaskTokenUrlHelper;

        public MemberController(
            IAdvancedContentRepository contentRepository,
            IAuthorizedTaskTokenUrlHelper authorizedTaskTokenUrlHelper
            )
        {
            _contentRepository = contentRepository;
            _authorizedTaskTokenUrlHelper = authorizedTaskTokenUrlHelper;
        }

        [Route("register")]
        public async Task<IActionResult> Register()
        {
            // Init an empty view model to avoid returning null
            var viewModel = new RegisterNewUserViewModel();

            // First validate that the current user isn't signed in
            var isSignedIn = await _contentRepository
                .Users()
                .Current()
                .IsSignedIn()
                .ExecuteAsync();

            if (isSignedIn)
            {
                ModelState.AddModelError(string.Empty, "You cannot register because you are already signed in.");
                return View(viewModel);
            }

            // We use IAuthorizedTaskTokenUrlHelper to get the token from the query
            // however is optional and you can pass the token in any way you want
            var token = _authorizedTaskTokenUrlHelper.ParseTokenFromQuery(this.Request.Query);

            // Validating the token will return a result that describes any errors
            var result = await _contentRepository
                .AuthorizedTasks()
                .ValidateAsync(new ValidateAuthorizedTaskTokenQuery()
                {
                    AuthorizedTaskTypeCode = MemberInviteAuthorizedTaskType.Code,
                    Token = token
                })
                .ExecuteAsync();

            // If not successful, add the error message to the ModelState.
            // Alternatively you can return the full error model to the view
            // or call result.ThrowIfNotSuccess() to throw an exception.
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error.Message);
            }

            return View(viewModel);
        }
    }
}