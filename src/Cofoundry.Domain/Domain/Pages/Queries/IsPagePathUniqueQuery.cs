using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsPagePathUniqueQuery : IQuery<bool>
    {
        public int? PageId { get; set; }
        public string UrlPath { get; set; }
        public int WebDirectoryId { get; set; }
        public int? LocaleId { get; set; }
    }
}
