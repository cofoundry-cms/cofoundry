using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// An entity that stores the date is was created.
    /// </summary>
    public interface ICreateable
    {
        /// <summary>
        /// Date at which the entity was created.
        /// </summary>
        DateTime CreateDate { get; set; }
    }
}
