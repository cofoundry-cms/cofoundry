using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsWebDirectoryPathUniqueQuery : IQuery<bool>
    {
        public int? WebDirectoryId { get; set; }
        public int? ParentWebDirectoryId { get; set; }
        public string UrlPath { get; set; }
    }
}
