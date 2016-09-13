using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SearchDocumentAssetSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<DocumentAssetSummary>>
    {
        public string Tags { get; set; }

        public string FileExtension { get; set; }

        public string FileExtensions { get; set; }
    }
}
