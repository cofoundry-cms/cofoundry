using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetActiveLocaleByIETFLanguageTagQuery : IQuery<ActiveLocale>
    {
        #region constructors

        public GetActiveLocaleByIETFLanguageTagQuery()
        {
        }

        public GetActiveLocaleByIETFLanguageTagQuery(string tag)
        {
            IETFLanguageTag = tag;
        }

        #endregion

        [Required]
        public string IETFLanguageTag { get; set; }
    }
}
