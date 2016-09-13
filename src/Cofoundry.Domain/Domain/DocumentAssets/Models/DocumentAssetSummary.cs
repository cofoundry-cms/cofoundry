using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class DocumentAssetSummary : IUpdateAudited, IDocumentAssetRenderable
    {
        public int DocumentAssetId { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string Title { get; set; }

        public long FileSizeInBytes { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public UpdateAuditData AuditData { get; set; }
    }
}
