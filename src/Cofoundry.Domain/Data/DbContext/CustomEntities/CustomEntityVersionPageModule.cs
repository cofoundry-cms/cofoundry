using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityVersionPageModule : IEntityOrderable, IEntityVersionPageModule
    {
        public int CustomEntityVersionPageModuleId { get; set; }

        public int CustomEntityVersionId { get; set; }

        public int PageTemplateSectionId { get; set; }

        public int PageModuleTypeId { get; set; }

        public int? PageModuleTypeTemplateId { get; set; }

        public string SerializedData { get; set; }

        public int Ordering { get; set; }

        public virtual CustomEntityVersion CustomEntityVersion { get; set; }

        public virtual PageTemplateSection PageTemplateSection { get; set; }

        public virtual PageModuleType PageModuleType { get; set; }

        public virtual PageModuleTypeTemplate PageModuleTypeTemplate { get; set; }

        public int GetVersionModuleId()
        {
            return CustomEntityVersionPageModuleId;
        }
    }
}
