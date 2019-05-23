using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class NestedDataModelMultiTypeItem
    {
        public string TypeName { get; set; }

        public INestedDataModel DataModel { get; set; }
    }
}
