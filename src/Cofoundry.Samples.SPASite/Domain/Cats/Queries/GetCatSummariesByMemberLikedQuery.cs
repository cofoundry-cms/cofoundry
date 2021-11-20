using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    public class GetCatSummariesByMemberLikedQuery : IQuery<ICollection<CatSummary>>
    {
        public GetCatSummariesByMemberLikedQuery() {}

        public GetCatSummariesByMemberLikedQuery(int id)
        {
            UserId = id;
        }

        public int UserId { get; set; }
    }
}
