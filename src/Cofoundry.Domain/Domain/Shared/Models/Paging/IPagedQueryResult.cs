using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IPagedQueryResult
    {
        int TotalItems { get; set; }

        int PageCount { get; set; }

        int PageNumber { get; set; }

        int PageSize { get; set; }
    }

    public interface IPagedQueryResult<TResult> : IPagedQueryResult
    {
        ICollection<TResult> Items { get; set; }
    }
}
