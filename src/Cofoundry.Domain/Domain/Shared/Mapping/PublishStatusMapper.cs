using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
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
            switch (code)
            {
                case PublishStatusCode.Published:
                    return PublishStatus.Published;
                case PublishStatusCode.Unpublished:
                    return PublishStatus.Unpublished;
                default:
                    throw new Exception("PublishStatusCode not recognised: " + code);
            }
        }

        /// <summary>
        /// Maps adomain publish status enum to a database code.
        /// </summary>
        /// <param name="publishStatus">Enum to map.</param>
        public static string ToCode(PublishStatus publishStatus)
        {
            switch (publishStatus)
            {
                case PublishStatus.Published:
                    return PublishStatusCode.Published;
                case PublishStatus.Unpublished:
                    return PublishStatusCode.Unpublished;
                default:
                    throw new Exception("PublishStatus not recognised: " + publishStatus);
            }
        }
    }
}
