﻿namespace Cofoundry.Domain;

/// <summary>
/// Query to get a collection of all custom entity definitions registered
/// with the system. The returned projections contain much of the same data 
/// as the source defintion class, but the main difference is that instead of 
/// using generics to identify the data model type, there is instead a 
/// DataModelType property.
/// </summary>
public class GetAllCustomEntityDefinitionSummariesQuery : IQuery<IReadOnlyCollection<CustomEntityDefinitionSummary>>
{
}
