using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class PageQueryExtensions
    {
        public static IQueryable<Page> FilterById(this IQueryable<Page> pages, int id)
        {
            var result = pages
                .Where(i => i.PageId == id && !i.IsDeleted);

            return result;
        }
    }
}
