using Cofoundry.Core.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper for resolving urls to static files.
    /// </summary>
    public interface IStaticFileViewHelper
    {
        /// <summary>
        /// Appends a version hash querystring parameter to the end
        /// of the file path, e.g. 'myfile.js?v=examplecomputedhash'
        /// </summary>
        /// <param name="applicationRelativePath">
        /// The static resource file path, which must be the full application 
        /// relative path.
        /// </param>
        /// <returns>
        /// If the file is found, the path is returned with the version 
        /// appended, otherwise the unmodified path is returned.
        /// </returns>
        string AppendVersion(string applicationRelativePath);

        /// <summary>
        /// Maps a relative static file url to an absolute one with a version parameter.
        /// </summary>
        /// <param name="applicationRelativePath">
        /// The static resource file path, which must be the full application 
        /// relative path.
        /// </param>
        /// <returns>
        /// If the file is found, the path is resolved with the version 
        /// appended, otherwise the resolved path is returned.
        /// </returns>
        string ToAbsoluteWithFileVersion(string applicationRelativePath);
    }
}
