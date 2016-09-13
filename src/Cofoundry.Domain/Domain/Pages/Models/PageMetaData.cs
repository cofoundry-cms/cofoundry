using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageMetaData
    {
        public PageMetaData()
        {
        }

        public PageMetaData(string keywords, string description)
        {
            Keywords = keywords;
            Description = description;
        }

        public string Keywords { get; set; }
        public string Description { get; set; }
    }
}
