using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SearchImageAssetSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<ImageAssetSummary>>
    {
        public string Tags { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public int? MinWidth { get; set; }

        public int? MinHeight { get; set; }
    }
}
