using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntityVersionSummary objects.
    /// </summary>
    public interface ICustomEntityVersionSummaryMapper
    {
        /// <summary>
        /// Maps an EF CustomEntityVersion record from the db into a CustomEntityVersionSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbVersion">CustomEntityVersion record from the database</param>
        CustomEntityVersionSummary Map(CustomEntityVersion dbVersion);
    }
}
