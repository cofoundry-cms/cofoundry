using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDetailsByIdQuery : IQuery<CustomEntityDetails>
    {
        public GetCustomEntityDetailsByIdQuery()
        {
        }

        public GetCustomEntityDetailsByIdQuery(int customEntityId)
        {
            CustomEntityId = customEntityId;
        }

        public int CustomEntityId { get; set; }
    }
}
