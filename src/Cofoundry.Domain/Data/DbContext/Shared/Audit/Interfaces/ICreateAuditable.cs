using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Marks an Entity Framework entity that has audit data 
    /// for entity creation.
    /// </summary>
    public interface ICreateAuditable : ICreateable
    {
        /// <summary>
        /// The user that created the entity.
        /// </summary>
        User Creator { get; set; }

        /// <summary>
        /// The database id of the user that created the entity.
        /// </summary>
        int CreatorId { get; set; }
    }
}
