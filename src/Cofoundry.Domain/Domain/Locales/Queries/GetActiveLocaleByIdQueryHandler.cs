using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class GetActiveLocaleByIdQueryHandler 
        : IQueryHandler<GetActiveLocaleByIdQuery, ActiveLocale>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetActiveLocaleByIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }
        
        public async Task<ActiveLocale> ExecuteAsync(GetActiveLocaleByIdQuery query, IExecutionContext executionContext)
        {
            var locales = await _queryExecutor.ExecuteAsync(new GetAllActiveLocalesQuery(), executionContext);
            var result = locales.SingleOrDefault(l => l.LocaleId == query.LocaleId);

            return result;
        }
    }
}
