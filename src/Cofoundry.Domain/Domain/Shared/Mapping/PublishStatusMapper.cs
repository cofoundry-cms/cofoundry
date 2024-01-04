using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Mapping helper for mapping between the PublishStatus enum and the 
/// publish status code used in the database.
/// </summary>
public static class PublishStatusMapper
{
    /// <summary>
    /// Maps a database publish status code to the domain enum.
    /// </summary>
    /// <param name="code">Database publish status code to map.</param>
    public static PublishStatus FromCode(string code)
    {
        return code switch
        {
            PublishStatusCode.Published => PublishStatus.Published,
            PublishStatusCode.Unpublished => PublishStatus.Unpublished,
            _ => throw new Exception("PublishStatusCode not recognised: " + code),
        };
    }

    /// <summary>
    /// Maps adomain publish status enum to a database code.
    /// </summary>
    /// <param name="publishStatus">Enum to map.</param>
    public static string ToCode(PublishStatus publishStatus)
    {
        return publishStatus switch
        {
            PublishStatus.Published => PublishStatusCode.Published,
            PublishStatus.Unpublished => PublishStatusCode.Unpublished,
            _ => throw new Exception("PublishStatus not recognised: " + publishStatus),
        };
    }
}
