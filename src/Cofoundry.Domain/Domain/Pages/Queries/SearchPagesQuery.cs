using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SearchPagesQuery : IQuery<IEnumerable<PageSearchResult>>
    {
        public string Text { get; set; }
        public int? LocaleId { get; set; }
    }
}
