﻿namespace Cofoundry.Domain;

/// <summary>
/// Query to return a collection of all user areas registered with the system
/// as a lightweight <see cref="UserAreaMicroSummary"/> projection, which 
/// contains only basic identification properties such as code and name.
/// </summary>
public class GetAllUserAreaMicroSummariesQuery : IQuery<IReadOnlyCollection<UserAreaMicroSummary>>
{
}
