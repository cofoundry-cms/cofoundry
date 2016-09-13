using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageModuleType
    {
        public PageModuleType()
        {
            PageModuleTemplates = new List<PageModuleTypeTemplate>();
            PageVersionModules = new List<PageVersionModule>();
            PageTemplateSections = new List<PageTemplateSection>();
        }

        public int PageModuleTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public bool IsCustom { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual ICollection<PageModuleTypeTemplate> PageModuleTemplates { get; set; }
        public virtual ICollection<PageVersionModule> PageVersionModules { get; set; }
        public virtual ICollection<PageTemplateSection> PageTemplateSections { get; set; }
    }
}
