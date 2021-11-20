using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    public class GetBreedByIdQuery : IQuery<Breed>
    {
        public GetBreedByIdQuery() {}

        public GetBreedByIdQuery(int id)
        {
            BreedId = id;
        }

        public int BreedId { get; set; }
    }
}
