using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Abstraction to allow CustomEntityPageModule and PageVersionModule to
    /// be treated as the same thing.
    /// </summary>
    public interface IEntityVersionPageModule
    {
        int PageTemplateSectionId { get; set; }

        int PageModuleTypeId { get; set; }

        int? PageModuleTypeTemplateId { get; set; }

        string SerializedData { get; set; }

        int Ordering { get; set; }

        PageTemplateSection PageTemplateSection { get; set; }

        PageModuleType PageModuleType { get; set; }

        PageModuleTypeTemplate PageModuleTypeTemplate { get; set; }

        int GetVersionModuleId();
    }
}
