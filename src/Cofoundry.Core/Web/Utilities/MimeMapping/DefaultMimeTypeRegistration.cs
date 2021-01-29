using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web.Internal
{
    /// <summary>
    /// Registers a few additional mime types missing from the asp.net FileExtensionContentTypeProvider
    /// </summary>
    public class DefaultMimeTypeRegistration : IMimeTypeRegistration
    {
        public void Register(IMimeTypeRegistrationContext context)
        {
            context.AddOrUpdate(".odp", "application/vnd.oasis.opendocument.presentation");
            context.AddOrUpdate(".ods", "application/vnd.oasis.opendocument.spreadsheet");
            context.AddOrUpdate(".odt", "application/vnd.oasis.opendocument.text");
            context.AddOrUpdate(".weba", "audio/webm");
            context.AddOrUpdate(".epub", "application/epub+zip");
            context.AddOrUpdate(".azw", "application/vnd.amazon.ebook");
            context.AddOrUpdate(".csv", "text/csv");
        }
    }
}
