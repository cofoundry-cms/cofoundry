using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// CustomEntityDefinitions are definied in code and stored in the database, so if they are missing
    /// from the databse we need to add them. Execute this to ensure that the custom entity definition has been saved
    /// to the database before assigning it to another entity.
    /// </summary>
    public class EnsureCustomEntityDefinitionExistsCommand : ICommand
    {
        /// <summary>
        /// CustomEntityDefinitions are definied in code and stored in the database, so if they are missing
        /// from the databse we need to add them. Execute this to ensure that the custom entity definition has been saved
        /// to the database before assigning it to another entity.
        /// </summary>
        public EnsureCustomEntityDefinitionExistsCommand()
        {
        }

        /// <summary>
        /// CustomEntityDefinitions are definied in code and stored in the database, so if they are missing
        /// from the databse we need to add them. Execute this to ensure that the custom entity definition has been saved
        /// to the database before assigning it to another entity.
        /// </summary>
        /// <param name="customEntityDefinitionCode">
        /// Unique 6 character definition code of the custom entity type
        /// to run the command on.
        /// </param>
        public EnsureCustomEntityDefinitionExistsCommand(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        /// <summary>
        /// Unique 6 character definition code of the custom entity type
        /// to run the command on.
        /// </summary>
        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }
    }
}
