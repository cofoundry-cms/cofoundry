using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.IO;

namespace Cofoundry.Domain
{
    public class GetCurrentActiveLocaleQueryHandler 
        : IQueryHandler<GetCurrentActiveLocaleQuery, ActiveLocale>
        , IAsyncQueryHandler<GetCurrentActiveLocaleQuery, ActiveLocale>
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


        public ActiveLocale Execute(GetCurrentActiveLocaleQuery query, IExecutionContext executionContext)
        {
            var result = _queryExecutor.Execute(GetQuery());

            return result;
        }

        public async Task<ActiveLocale> ExecuteAsync(GetCurrentActiveLocaleQuery query, IExecutionContext executionContext)
        {
            var result = await _queryExecutor.ExecuteAsync(GetQuery());

            return result;
        }

        private GetActiveLocaleByIETFLanguageTagQuery GetQuery()
        {
            var tag = _cultureContextService.GetCurrent().Name;
            return new GetActiveLocaleByIETFLanguageTagQuery(tag);
        }
    }
}
