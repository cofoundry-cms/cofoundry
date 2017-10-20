using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCurrentActiveLocaleQueryHandler 
        : IAsyncQueryHandler<GetCurrentActiveLocaleQuery, ActiveLocale>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICultureContextService _cultureContextService;

        public GetCurrentActiveLocaleQueryHandler(
            IQueryExecutor queryExecutor,
            ICultureContextService cultureContextService
            )
        {
            _queryExecutor = queryExecutor;
            _cultureContextService = cultureContextService;
        }

        public async Task<ActiveLocale> ExecuteAsync(GetCurrentActiveLocaleQuery query, IExecutionContext executionContext)
        {
            var tag = _cultureContextService.GetCurrent()?.Name;

            if (string.IsNullOrWhiteSpace(tag)) return null;

            var byTagQuery = new GetActiveLocaleByIETFLanguageTagQuery(tag);
            var result = await _queryExecutor.ExecuteAsync(byTagQuery);

            return result;
        }
    }
}
