using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    public class SearchBlogPostsQuery : SimplePageableQuery
    {
        public int CategoryId { get; set; }
    }
}