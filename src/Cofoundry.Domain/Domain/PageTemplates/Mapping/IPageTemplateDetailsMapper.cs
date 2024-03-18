﻿using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to PageTemplateDetails objects.
/// </summary>
public interface IPageTemplateDetailsMapper
{
    /// <summary>
    /// Maps an EF PageTemplate record from the db into an PageTemplateDetailsMapper 
    /// object. If the db record is null then null is returned.
    /// </summary>
    /// <param name="queryModel">Query data returned from the database.</param>
    PageTemplateDetails? Map(PageTemplateDetailsQueryModel? queryModel);
}
