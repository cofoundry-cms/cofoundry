using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Used to turn relative paths into absolute urls e.g.
    /// /mypage.htm into http://www.mysite/mypage.htm
    /// </summary>
    public interface ISiteUrlResolver
    {
        /// <summary>
        /// Maps a relative path to an absolute url e.g.
        /// /mypage.htm into http://www.mysite/mypage.htm
        /// </summary>
        /// <param name="path">Path to resolve</param>
        /// <returns>The absolute path, or an empty string if the supplied path is null or empty.</returns>
        string MakeAbsolute(string path);

        /// <summary>
        /// Maps a relative path to an absolute path
        /// </summary>
        /// <param name="path">path to resolve</param>
        /// <param name="forceSsl">whether to make the new uri https. If this is false then the scheme is not modified</param>
        /// <returns>The absolute path, or an empty string if the supplied path is null or empty.</returns>
        string MakeAbsolute(string path, bool forceSsl);
    }
}
