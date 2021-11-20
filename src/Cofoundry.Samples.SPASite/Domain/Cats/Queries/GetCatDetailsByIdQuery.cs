using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    public class GetCatDetailsByIdQuery : IQuery<CatDetails>
    {
        public GetCatDetailsByIdQuery() {}

        public GetCatDetailsByIdQuery(int id)
        {
            CatId = id;
        }

        public int CatId { get; set; }
    }
}
