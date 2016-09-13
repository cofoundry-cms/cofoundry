using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class DeleteUnstructuredDataDependenciesCommand : ICommand
    {
        public DeleteUnstructuredDataDependenciesCommand(string rootEntityDefinitionCode, int rootEntityId)
        {
            RootEntityDefinitionCode = rootEntityDefinitionCode;
            RootEntityId = rootEntityId;
        }

        [Required]
        public string RootEntityDefinitionCode { get; set; }

        [Required]
        [PositiveInteger]
        public int RootEntityId { get; set; }
    }
}
