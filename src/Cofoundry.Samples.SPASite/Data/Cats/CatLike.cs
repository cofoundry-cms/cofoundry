using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Data
{
    public class CatLike
    {
        public int CatCustomEntityId { get; set; }

        public int UserId { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual User User { get; set; }

        public virtual CustomEntity CatCustomEntity { get; set; }
    }
}
