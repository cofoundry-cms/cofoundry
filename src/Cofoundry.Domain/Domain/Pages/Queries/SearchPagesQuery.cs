using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Full-text page searching has not been looked at yet and this should not be used, but
    /// remains in place for compatibility.
    /// </summary>
    public class SearchPagesQuery : IQuery<ICollection<PageSearchResult>>
    {
        public string Text { get; set; }
        public int? LocaleId { get; set; }
    }
}
