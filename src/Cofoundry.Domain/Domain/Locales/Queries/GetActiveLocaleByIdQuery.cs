using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetActiveLocaleByIdQuery : IQuery<ActiveLocale>
    {
        public GetActiveLocaleByIdQuery()
        {
        }

        public GetActiveLocaleByIdQuery(int localeId)
        {
            LocaleId = localeId;
        }

        public int LocaleId { get; set; }
    }
}
