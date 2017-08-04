using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageBlockTypeTemplate
    {
        public int PageBlockTypeTemplateId { get; set; }
        public int PageBlockTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public virtual PageBlockType PageBlockType { get; set; }
        
        public DateTime CreateDate { get; set; }
    }
}
