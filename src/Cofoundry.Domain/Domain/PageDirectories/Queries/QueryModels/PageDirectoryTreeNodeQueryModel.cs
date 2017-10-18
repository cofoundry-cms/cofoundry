using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.QueryModels
{
    public class PageDirectoryTreeNodeQueryModel
    {
        public PageDirectory PageDirectory { get; set; }
        public User Creator { get; set; }
        public int NumPages { get; set; }
    }
}
