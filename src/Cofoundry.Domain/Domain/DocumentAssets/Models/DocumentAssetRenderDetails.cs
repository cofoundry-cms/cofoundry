using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class DocumentAssetRenderDetails : IDocumentAssetRenderable
    {
        public int DocumentAssetId { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public long FileSizeInBytes { get; set; }
    }
}
