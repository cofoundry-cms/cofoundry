using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class Tag
    {
        public int TagId { get; set; }
        public string TagText { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
