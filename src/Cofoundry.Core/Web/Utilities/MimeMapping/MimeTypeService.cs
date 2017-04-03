using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web
{
    /// <summary>
    /// Service for working with mime types.
    /// </summary>
    public class MimeTypeService : IMimeTypeService
    {
        /// <summary>
        /// Finds a mime type that matches the file extension in a file name. Equivalent to 
        /// the old MimeMapping.GetMimeMapping method from .NET 4.x
        /// </summary>
        /// <param name="fileName">File name with file extension (path optional)</param>
        public string MapFromFileName(string fileName)
        {
            return MimeMapping.GetMimeMapping(fileName);
        }
    }
}
