using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IDocumentAssetRenderable
    {
        int DocumentAssetId { get; set; }

        string FileName { get; set; }

        string FileExtension { get; set; }

        string Title { get; set; }

        long FileSizeInBytes { get; set; }
    }
}
