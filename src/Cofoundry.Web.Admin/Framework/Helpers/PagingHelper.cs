using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web.Admin
{
    public static class ApiPagingHelper
    {
        /// <summary>
        /// Sets the default bounds for admin paging queries which is a 
        /// default page size of 20 and and a maximum page size of 100.
        /// </summary>
        /// <param name="query">Query to set bounds for.</param>
        public static void SetDefaultBounds(IPageableQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            query.SetBounds(40, 100);
        }

    }
}
