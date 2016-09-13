using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Cofoundry.Web
{
    public class AntiCSRFService : IAntiCSRFService
    {
        public string GetToken()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return cookieToken + ":" + formToken;
        }

        public void ValidateToken(string token)
        {
            string cookieToken = string.Empty;
            string formToken = string.Empty;

            if (!string.IsNullOrWhiteSpace(token))
            {
                string[] tokens = token.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }

            AntiForgery.Validate(cookieToken, formToken);
        }
    }
}
