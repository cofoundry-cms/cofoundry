using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetAllActiveLocalesQueryHandler 
        : IQueryHandler<GetAllActiveLocalesQuery, ICollection<ActiveLocale>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ILocaleCache _cache;
        private readonly IActiveLocaleMapper _activeLocaleMapper;

        public GetAllActiveLocalesQueryHandler(
            CofoundryDbContext dbContext,
            ILocaleCache cache,
            IActiveLocaleMapper activeLocaleMapper
            )
        {
            _dbContext = dbContext;
            _cache = cache;
            _activeLocaleMapper = activeLocaleMapper;
        }

        #endregion

        #region execution

        public async Task<ICollection<ActiveLocale>> ExecuteAsync(GetAllActiveLocalesQuery query, IExecutionContext executionContext)
        {
            var results = await _cache.GetOrAddAsync(new Func<Task<ICollection<ActiveLocale>>>(async () =>
            {
                var dbResults = await GetAllLocales().ToListAsync();
                var mappedResults = dbResults
                    .Select(_activeLocaleMapper.Map)
                    .ToList();

                return mappedResults;
            }));

            return results;
        }

        #endregion

        #region helpers

        private IQueryable<Locale> GetAllLocales()
        {
            return _dbContext
                    .Locales
                    .AsNoTracking()
                    .Where(l => l.IsActive)
                    .OrderBy(l => l.LocaleName);
        }

        #endregion
    }
}
