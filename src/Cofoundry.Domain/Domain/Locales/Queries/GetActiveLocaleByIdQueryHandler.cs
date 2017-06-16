using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.IO;

namespace Cofoundry.Domain
{
    public class GetActiveLocaleByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<ActiveLocale>, ActiveLocale>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetActiveLocaleByIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }
        
        public async Task<ActiveLocale> ExecuteAsync(GetByIdQuery<ActiveLocale> query, IExecutionContext executionContext)
        {
            var result = (await _queryExecutor
                .GetAllAsync<ActiveLocale>())
                .SingleOrDefault(l => l.LocaleId == query.Id);

            return result;
        }
    }
}
