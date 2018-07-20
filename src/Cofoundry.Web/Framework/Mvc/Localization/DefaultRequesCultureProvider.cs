using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Framework.Mvc.Localization
{
    /// <summary>
    /// Represents a provider for determining the culture information of an <see cref="T:Microsoft.AspNetCore.Http.HttpRequest" />.
    /// </summary>
    public class DefaultRequesCultureProvider : RequestCultureProvider
    {
        /// <summary>
        /// Cookie Request Culture Provider
        /// </summary>
        public CookieRequestCultureProvider CookieRequestCultureProvider => Options.RequestCultureProviders
            .OfType<CookieRequestCultureProvider>().FirstOrDefault();

        /// <summary>
        /// Accept Language Culture Provider
        /// </summary>
        public AcceptLanguageHeaderRequestCultureProvider AcceptLanguageHeaderRequestCultureProvider => Options.RequestCultureProviders
            .OfType<AcceptLanguageHeaderRequestCultureProvider>().FirstOrDefault();

        /// <summary>
        /// Implements the provider to determine the culture of the given request.
        /// </summary>
        /// <param name="httpContext">The <see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> for the request.</param>
        /// <returns>
        ///     The determined <see cref="T:Microsoft.AspNetCore.Localization.ProviderCultureResult" />.
        ///     Returns <c>null</c> if the provider couldn't determine a <see cref="T:Microsoft.AspNetCore.Localization.ProviderCultureResult" />.
        /// </returns>
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var path = httpContext.Request.Path;
            var pageLocaleParser = httpContext.RequestServices.GetService<IPageLocaleParser>();
            var locale = await pageLocaleParser.ParseLocaleAsync(path);
            if (locale != null)
            {
                return new ProviderCultureResult(locale.IETFLanguageTag);
            }

            var result = NullProviderCultureResult.Result;

            if (CookieRequestCultureProvider != null)
            {
                result = await CookieRequestCultureProvider.DetermineProviderCultureResult(httpContext);
                if (result != NullProviderCultureResult.Result)
                {
                    return result;
                }
            }

            if (AcceptLanguageHeaderRequestCultureProvider != null)
            {
                result = await AcceptLanguageHeaderRequestCultureProvider.DetermineProviderCultureResult(httpContext);
            }
            return result != NullProviderCultureResult.Result ? result : new ProviderCultureResult(Options.DefaultRequestCulture.Culture.Name);
            
        }

    }
}