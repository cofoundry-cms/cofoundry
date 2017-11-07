using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class CustomEntityPublishStatusQuery
    {
        public int CustomEntityId { get; set; }

        public short PublishStatusQueryId { get; set; }

        public int CustomEntityVersionId { get; set; }

        public virtual CustomEntity CustomEntity { get; set; }

        public virtual CustomEntityVersion CustomEntityVersion { get; set; }

    }
}
