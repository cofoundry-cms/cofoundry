using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;
using AutoMapper;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns all page directories as PageDirectoryRoute objects. The results of this 
    /// query are cached.
    /// </summary>
    public class GetAllPageDirectoryRoutesQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<PageDirectoryRoute>, IEnumerable<PageDirectoryRoute>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageDirectoryRoute>, IEnumerable<PageDirectoryRoute>>
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
        
        public async Task<IEnumerable<PageDirectoryRoute>> ExecuteAsync(GetAllQuery<PageDirectoryRoute> query, IExecutionContext executionContext)
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
                .Include(d => d.PageDirectoryLocales)
                .Where(d => d.IsActive);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<PageDirectoryRoute> command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
