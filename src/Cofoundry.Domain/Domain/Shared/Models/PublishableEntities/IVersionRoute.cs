namespace Cofoundry.Domain;

/// <summary>
/// Used to describe an object that contains dynamic page routing
/// information. Currently applied to a PageRoute and a CustomEntityRoute.
/// </summary>
public interface IVersionRoute
{
    /// <summary>
    /// The database identifier for this route version. The data
    /// used for this property depends on the implementation.
    /// </summary>
    int VersionId { get; }

    /// <summary>
    /// The workflow state of this version e.g. draft/published.
    /// </summary>
    WorkFlowStatus WorkFlowStatus { get; }

    /// <summary>
    /// A user friendly title of the version.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// A page can have many published versions, this flag indicates if
    /// it is the latest published version which displays on the live site
    /// when the page itself is published.
    /// </summary>
    bool IsLatestPublishedVersion { get; }

    /// <summary>
    /// Date that the version was created.
    /// </summary>
    DateTime CreateDate { get; }
}
