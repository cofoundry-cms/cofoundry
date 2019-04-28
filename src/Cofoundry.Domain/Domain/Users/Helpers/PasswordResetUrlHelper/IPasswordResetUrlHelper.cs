using Cofoundry.Domain.MailTemplates;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple helper for formatting a password reset url using
    /// a standard set of parameters.
    /// </summary>
    public interface IPasswordResetUrlHelper
    {
        /// <summary>
        /// Formats a password reset url using the parameters contained in
        /// the template builder context. Url is in the format 
        /// "{baseUrl}?i={userPasswordResetRequestId}&amp;t={token}"
        /// </summary>
        /// <param name="context">Context from the template builder containing the data to encode in the url.</param>
        /// <returns>The formatted url.</returns>
        string MakeUrl(PasswordResetRequestedByUserTemplateBuilderContext context);

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
        string MakeUrl(
            Uri baseUri,
            Guid userPasswordResetRequestId,
            string token
            );

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
        PasswordResetUrlParameters ParseFromQuery(IQueryCollection queryCollection);
    }
}
