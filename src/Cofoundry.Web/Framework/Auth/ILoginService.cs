using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public interface ILoginService
    {
        void LogAuthenticatedUserIn(int userId, bool rememberUser);
        void LogFailedLoginAttempt(string userAreaCode, string username);

        Task LogAuthenticatedUserInAsync(int userId, bool rememberUser);
        Task LogFailedLoginAttemptAsync(string userAreaCode, string username);

        void SignOut();
    }
}
