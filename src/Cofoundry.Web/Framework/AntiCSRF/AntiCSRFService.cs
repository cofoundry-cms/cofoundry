using Cofoundry.Core;
using Microsoft.AspNetCore.Antiforgery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Service for generating and validation tokens to prevent
    /// cross site request forgery attacks.
    /// </summary>
    public class AntiCSRFService : IAntiCSRFService
    {
        private readonly IAntiforgery _antiforgery;
        const char TOKEN_DELIMITER = ':';

        public AntiCSRFService(
            IAntiforgery antiforgery
            )
        {
            _antiforgery = antiforgery;
        }
        public string GetToken()
        {
            //string cookieToken, formToken;
            //AntiForgery.GetTokens(null, out cookieToken, out formToken);
            //return cookieToken + TOKEN_DELIMITER + formToken;
            return "TODO";
        }

        public void ValidateToken(string token)
        {
            string cookieToken = string.Empty;
            string formToken = string.Empty;

            if (!string.IsNullOrWhiteSpace(token))
            {
                // Asp.Net doesn't split up multiple values from the header and
                // for some reason an additional token is added to the header 
                // presumable by the browser of an extension and we need to ignore it
                // so here we strip any additional tokens and assume ours if the first.
                token = StringHelper
                    .SplitAndTrim(token, ',')
                    .First();

                string[] tokens = token.Split(TOKEN_DELIMITER);
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }

            // TODO
            //AntiForgery.Validate(cookieToken, formToken);
        }
    }
}
