using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a <see cref="UserMicroSummary"/> object representing the currently logged in 
    /// user. If the user is not logged in then <see langword="null"/> is returned. If  multiple user areas 
    /// are implemented, then the returned user will depend on the "ambient" auth scheme, which 
    /// is typically the default user area unless the ambient scheme has been changed during 
    /// the flow of the request e.g. via an AuthorizeUserAreaAttribute.
    /// </summary>
    public class GetCurrentUserMicroSummaryQuery : IQuery<UserMicroSummary>
    {
    }
}