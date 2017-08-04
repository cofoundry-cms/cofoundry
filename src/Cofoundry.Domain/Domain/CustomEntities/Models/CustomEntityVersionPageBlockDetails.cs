using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionPageBlockDetails
    {
        public int CustomEntityVersionPageBlockId { get; set; }

        public PageBlockTypeTemplateSummary Template { get; set; }

        public PageBlockTypeSummary BlockType { get; set; }

        public IPageBlockTypeDataModel DataModel { get; set; }
    }
}
