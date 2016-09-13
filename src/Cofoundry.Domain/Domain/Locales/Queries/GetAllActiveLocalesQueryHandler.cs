using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    public class GetAllActiveLocalesQueryHandler 
        : IQueryHandler<GetAllQuery<ActiveLocale>, IEnumerable<ActiveLocale>>
        , IAsyncQueryHandler<GetAllQuery<ActiveLocale>, IEnumerable<ActiveLocale>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ILocaleCache _cache;

        public GetAllActiveLocalesQueryHandler(
            CofoundryDbContext dbContext,
            ILocaleCache cache
            )
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        #endregion

        #region execution

        public IEnumerable<ActiveLocale> Execute(GetAllQuery<ActiveLocale> query, IExecutionContext executionContext)
        {
            var results = _cache.GetOrAdd(() =>
            {
                return GetAllLocales().ToArray();
            });

            return results;
        }

        public async Task<IEnumerable<ActiveLocale>> ExecuteAsync(GetAllQuery<ActiveLocale> query, IExecutionContext executionContext)
        {
            var results = await _cache.GetOrAddAsync(new Func<Task<ActiveLocale[]>>(async () =>
            {
                return await GetAllLocales().ToArrayAsync();
            }));

            return results;
        }

        #endregion

        #region helpers

        private IQueryable<ActiveLocale> GetAllLocales()
        {
            return _dbContext
                    .Locales
                    .AsNoTracking()
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.LocaleName)
                    .ProjectTo<ActiveLocale>();
        }

        #endregion
    }
}
