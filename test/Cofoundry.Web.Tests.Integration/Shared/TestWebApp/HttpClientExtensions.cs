using Cofoundry.Domain.Tests.Integration.SeedData;

namespace Cofoundry.Web.Tests.Integration.TestWebApp;

public static class HttpClientExtensions
{
    /// <summary>
    /// Sends a sign in request to impersonate the specified user on the test
    /// server environment.
    /// </summary>
    /// <param name="userInfo">
    /// User to impersonate. The user can be associated with any user area.
    /// </param>
    public static Task ImpersonateUserAsync(this HttpClient httpClient, TestUserInfo userInfo)
    {
        return ImpersonateUserAsync(httpClient, userInfo.UserId);
    }

    /// <summary>
    /// Sends a sign in request to impersonate the specified user on the test
    /// server environment.
    /// </summary>
    /// <param name="userId">
    /// UserId of the user to impersonate. The user can be associated with
    /// any user area.
    /// </param>
    public static Task ImpersonateUserAsync(this HttpClient httpClient, int userId)
    {
        return httpClient.PostAsync($"/tests/users/impersonate/{userId}", null);
    }
}
