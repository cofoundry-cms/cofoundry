using Cofoundry.Core;
using Cofoundry.Domain.MailTemplates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple helper for formatting a password reset url using
    /// a standard set of parameters.
    /// </summary>
    public class PasswordResetUrlHelper : IPasswordResetUrlHelper
    {
        private const string ID_PARAM_NAME = "i";
        private const string TOKEN_PARAM_NAME = "t";

        /// <summary>
        /// Formats a password reset url using the parameters contained in
        /// the template builder context. Url is in the format 
        /// "{baseUrl}?i={userPasswordResetRequestId}&amp;t={token}"
        /// </summary>
        /// <param name="context">Context from the template builder containing the data to encode in the url.</param>
        /// <returns>The formatted url.</returns>
        public string MakeUrl(PasswordResetRequestedByUserTemplateBuilderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return MakeUrl(context.ResetUrlBase, context.UserPasswordResetRequestId, context.Token);
        }

        /// <summary>
        /// Formats a password reset url. Url is in the format 
        /// "{baseUrl}?i={userPasswordResetRequestId}&amp;t={token}"
        /// </summary>
        /// <param name="baseUri">
        /// The relative base path used to construct the reset url 
        /// e.g. new Uri("/auth/forgot-password").
        /// </param>
        /// <param name="userPasswordResetRequestId">
        /// A unique identifier required to authenticate when 
        /// resetting the password.
        /// </param>
        /// <param name="token">An token used to authenticate when resetting the password.</param>
        /// <returns>The formatted url.</returns>
        public string MakeUrl(
            Uri baseUri,
            Guid userPasswordResetRequestId,
            string token
            )
        {
            if (baseUri == null) throw new ArgumentNullException(nameof(baseUri));
            if (userPasswordResetRequestId == Guid.Empty) throw new ArgumentEmptyException(nameof(userPasswordResetRequestId));
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            var queryParams = new Dictionary<string, string>()
            {
                { ID_PARAM_NAME, userPasswordResetRequestId.ToString("N") },
                { TOKEN_PARAM_NAME, token }
            };

            var formattedUri = baseUri.ToString().TrimEnd('/');

            var url = QueryHelpers.AddQueryString(formattedUri, queryParams);

            return url;
        }

        /// <summary>
        /// Parses the password reset parameters from a querystring
        /// collection. The collection should come from a request with
        /// a url created using PasswordResetUrlHelper.MakeUrl().
        /// </summary>
        /// <param name="queryCollection">
        /// Querystring parameters, which can be accessed in a controller
        /// using "Request.Query" or via "HttpContext.Request.Query".
        /// </param>
        /// <returns>
        /// Parsed parameter set, which will never be null but may contain null
        /// properties if parsing failed.
        /// </returns>
        public PasswordResetUrlParameters ParseFromQuery(IQueryCollection queryCollection)
        {
            var result = new PasswordResetUrlParameters();

            if (queryCollection.ContainsKey(ID_PARAM_NAME) 
                && Guid.TryParse(queryCollection[ID_PARAM_NAME], out Guid userPasswordResetRequestId))
            {
                result.UserPasswordResetRequestId = userPasswordResetRequestId;
            }

            if (queryCollection.ContainsKey(TOKEN_PARAM_NAME))
            {
                result.Token = Uri.UnescapeDataString(queryCollection[TOKEN_PARAM_NAME]);
            }

            return result;
        }
    }
}
