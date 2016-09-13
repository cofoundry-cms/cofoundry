using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a PageVersionModule command that requires the
    /// dynamic data model
    /// </summary>
    public interface IPageVersionModuleDataModelCommand
    {
        IPageModuleDataModel DataModel { get; set; }
        int PageModuleTypeId { get; set; }
        int? PageModuleTypeTemplateId { get; set; }
    }
}
