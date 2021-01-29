using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns all page directories as PageDirectoryRoute objects. The results of this 
    /// query are cached.
    /// </summary>
    public class GetAllPageDirectoryRoutesQueryHandler 
        : IQueryHandler<GetAllPageDirectoryRoutesQuery, ICollection<PageDirectoryRoute>>
        , IPermissionRestrictedQueryHandler<GetAllPageDirectoryRoutesQuery, ICollection<PageDirectoryRoute>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageDirectoryRouteMapper _pageDirectoryRouteMapper;

        public GetAllPageDirectoryRoutesQueryHandler(
            CofoundryDbContext dbContext,
            IPageDirectoryRouteMapper pageDirectoryRouteMapper
            )
        {
            _dbContext = dbContext;
            _pageDirectoryRouteMapper = pageDirectoryRouteMapper;
        }

        #endregion

        #region execution
        
        public async Task<ICollection<PageDirectoryRoute>> ExecuteAsync(GetAllPageDirectoryRoutesQuery query, IExecutionContext executionContext)
        {
            var dbPageDirectories = await Query().ToListAsync();
            var activeWebRoutes = _pageDirectoryRouteMapper.Map(dbPageDirectories);

            return activeWebRoutes;
        }
        
        private IQueryable<PageDirectory> Query()
        {
            return _dbContext
                .PageDirectories
                .AsNoTracking()
                .Include(d => d.PageDirectoryLocales);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageDirectoryRoutesQuery command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
