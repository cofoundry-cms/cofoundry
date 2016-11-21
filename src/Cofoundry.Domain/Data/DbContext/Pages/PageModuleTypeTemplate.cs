using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageModuleTypeTemplate
    {
        public int PageModuleTypeTemplateId { get; set; }
        public int PageModuleTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public virtual PageModuleType PageModuleType { get; set; }
        
        public DateTime CreateDate { get; set; }
    }
}
