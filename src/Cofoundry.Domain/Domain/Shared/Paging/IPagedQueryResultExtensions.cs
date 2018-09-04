using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class IPagedQueryResultExtensions
    {
        #region IPagedQueryResult

        public static bool IsFirstPage(this IPagedQueryResult source)
        {
            return source.PageNumber <= 1;
        }

        public static bool IsLastPage(this IPagedQueryResult source)
        {
            return source.PageCount <= source.PageNumber;
        }

        #endregion
    }
}
