using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to get a collection of all custom entity definitions registered
    /// with the system. The returned object is a lightweight projection of the
    /// data defined in a custom entity definition class which is typically used 
    /// as part of another domain model.
    /// </summary>
    public class GetAllCustomEntityDefinitionMicroSummariesQuery : IQuery<ICollection<CustomEntityDefinitionMicroSummary>>
    {
    }
}
