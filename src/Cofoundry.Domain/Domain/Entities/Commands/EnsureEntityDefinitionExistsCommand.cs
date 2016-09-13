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
    /// EntityDefinitions are definied in code and stored in the database, so if they are missing
    /// from the databse we need to add them. Execute this to ensure that the entity definition has been saved
    /// to the database before assigning it to another entity.
    /// </summary>
    public class EnsureEntityDefinitionExistsCommand : ICommand
    {
        public EnsureEntityDefinitionExistsCommand()
        {
        }

        public EnsureEntityDefinitionExistsCommand(string entityDefinitionCode)
        {
            EntityDefinitionCode = entityDefinitionCode;
        }

        [Required]
        [MaxLength(6)]
        public string EntityDefinitionCode { get; set; }
    }
}
