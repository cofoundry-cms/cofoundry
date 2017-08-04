using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a PageVersionBlock command that requires the
    /// dynamic data model
    /// </summary>
    public interface IPageVersionBlockDataModelCommand
    {
        IPageBlockTypeDataModel DataModel { get; set; }
        int PageBlockTypeId { get; set; }
        int? PageBlockTypeTemplateId { get; set; }
    }
}
