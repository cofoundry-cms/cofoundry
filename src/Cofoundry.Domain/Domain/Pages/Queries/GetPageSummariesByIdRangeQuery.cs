using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds pages with the specified page ids and returns them as PageSummary 
    /// objects. Note that this query does not account for WorkFlowStatus and so
    /// pages will be returned irrecpective of whether they aree published or not.
    /// </summary>
    public class GetPageSummariesByIdRangeQuery : IQuery<IDictionary<int, PageSummary>>
    {
        public GetPageSummariesByIdRangeQuery() { }

        /// <summary>
        /// Initializes the query with parameters
        /// </summary>
        /// <param name="pageIds">A collection of database ids of the pages to fetch.</param>
        public GetPageSummariesByIdRangeQuery(IEnumerable<int> pageIds)
        {
            PageIds = pageIds.ToArray();
        }

        /// <summary>
        /// A collection of database ids of the pages to fetch.
        /// </summary>
        public int[] PageIds { get; set; }
    }
}
