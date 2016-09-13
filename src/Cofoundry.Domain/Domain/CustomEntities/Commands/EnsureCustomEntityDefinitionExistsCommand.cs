using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// CustomEntityDefinitions are definied in code and stored in the database, so if they are missing
    /// from the databse we need to add them. Execute this to ensure that the custom entity definition has been saved
    /// to the database before assigning it to another entity.
    /// </summary>
    public class EnsureCustomEntityDefinitionExistsCommand : ICommand
    {
        public EnsureCustomEntityDefinitionExistsCommand()
        {
        }

        public EnsureCustomEntityDefinitionExistsCommand(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }
    }
}
