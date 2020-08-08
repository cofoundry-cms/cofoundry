using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.Text.RegularExpressions;

namespace Cofoundry.Domain.Internal
{
    public class GetActiveLocaleByIETFLanguageTagQueryHandler 
        : IQueryHandler<GetActiveLocaleByIETFLanguageTagQuery, ActiveLocale>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetActiveLocaleByIETFLanguageTagQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<ActiveLocale> ExecuteAsync(GetActiveLocaleByIETFLanguageTagQuery query, IExecutionContext executionContext)
        {
            if (!IsTagValid(query.IETFLanguageTag)) return null;

            var locales = await _queryExecutor.ExecuteAsync(new GetAllActiveLocalesQuery(), executionContext);
            var result = locales.SingleOrDefault(l => l.IETFLanguageTag.Equals(query.IETFLanguageTag, StringComparison.OrdinalIgnoreCase));

            return result;
        }

        private bool IsTagValid(string languageTag)
        {
            return Regex.IsMatch(languageTag, "^[a-zA-Z]{2}(-[a-zA-Z]{2})?$");
        }
    }
}
