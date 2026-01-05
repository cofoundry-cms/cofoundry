using Microsoft.AspNetCore.Antiforgery;

namespace SPASite;

[Route("api/auth")]
[AutoValidateAntiforgeryToken]
public class AuthApiController : ControllerBase
{
    private readonly IApiResponseHelper _apiResponseHelper;
    private readonly IAntiforgery _antiforgery;
    private readonly IDomainRepository _domainRepository;

    public AuthApiController(
        IApiResponseHelper apiResponseHelper,
        IAntiforgery antiforgery,
        IDomainRepository domainRepository)
    {
        _apiResponseHelper = apiResponseHelper;
        _antiforgery = antiforgery;
        _domainRepository = domainRepository;
    }

    /// <summary>
    /// Once we have logged in we need to re-fetch the csrf token because
    /// the user identity will have changed and the old token will be invalid
    /// </summary>
    [HttpGet("session")]
    public async Task<JsonResult> GetAuthSession()
    {
        return await _apiResponseHelper.RunWithResultAsync(async () =>
        {
            var member = await _domainRepository.ExecuteQueryAsync(new GetCurrentMemberSummaryQuery());
            var token = _antiforgery.GetAndStoreTokens(HttpContext);

            var sessionInfo = new
            {
                Member = member,
                AntiForgeryToken = token.RequestToken
            };

            return sessionInfo;
        });
    }

    [HttpPost("register")]
    public async Task<JsonResult> Register([FromBody] RegisterMemberAndSignInCommand command)
    {
        return await _apiResponseHelper.RunCommandAsync(command);
    }

    [HttpPost("login")]
    public async Task<JsonResult> Login([FromBody] SignMemberInCommand command)
    {
        return await _apiResponseHelper.RunCommandAsync(command);
    }

    [HttpPost("sign-out")]
    public async Task<JsonResult> SignOutUser()
    {
        var command = new SignMemberOutCommand();

        return await _apiResponseHelper.RunCommandAsync(command);
    }
}
