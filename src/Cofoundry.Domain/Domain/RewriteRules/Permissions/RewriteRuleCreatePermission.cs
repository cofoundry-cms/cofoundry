using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class RewriteRuleCreatePermission : IEntityPermission
    {
        public RewriteRuleCreatePermission()
        {
            EntityDefinition = new RewriteRuleEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Rewrite Rules");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
