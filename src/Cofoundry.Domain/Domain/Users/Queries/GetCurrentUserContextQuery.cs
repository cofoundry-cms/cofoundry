using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Gets basic information about the currently logged in user. If the user is not 
    /// logged in then <see cref="UserContext.Empty"/> is returned. If multiple user
    /// areas are implemented, then the returned user will depend on the "ambient" 
    /// auth scheme, which is typically the default user area unless the ambient scheme 
    /// has been changed during the flow of the request e.g. via an AuthorizeUserAreaAttribute.
    /// </para>
    /// <para>
    /// By default the <see cref="IUserContext"/> is cached for the lifetime of the service 
    /// (per request in web apps).
    /// </para>
    /// </summary>
    public class GetCurrentUserContextQuery : IQuery<IUserContext>
    {
        /// <summary>
        /// If your project uses multiple user areas, then you can optionally specify which user 
        /// area to return the user for. If <see cref="UserAreaCode"/> is not specified then the 
        /// returned user will depend on the "ambient" auth scheme, which is typically the default 
        /// user area unless the ambient scheme has been changed during the flow of the request 
        /// e.g. via an AuthorizeUserAreaAttribute.
        /// </summary>
        [StringLength(3)]
        public string UserAreaCode { get; set; }
    }
}