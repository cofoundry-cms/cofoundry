using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageVersionModule : ICreateAuditable, IEntityOrderable, IEntityVersionPageModule
    {
        public int PageVersionModuleId { get; set; }
        public int PageVersionId { get; set; }
        public int PageTemplateSectionId { get; set; }
        public int PageModuleTypeId { get; set; }
        public string SerializedData { get; set; }
        public int Ordering { get; set; }
        public DateTime UpdateDate { get; set; }
        public int? PageModuleTypeTemplateId { get; set; }

        public virtual PageTemplateSection PageTemplateSection { get; set; }
        public virtual PageModuleType PageModuleType { get; set; }
        public virtual PageVersion PageVersion { get; set; }
        public virtual PageModuleTypeTemplate PageModuleTypeTemplate { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion

        public int GetVersionModuleId()
        {
            return PageVersionModuleId;
        }
    }
}
