using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class SeoSettingsUpdatePermission : IEntityPermission
    {
        public SeoSettingsUpdatePermission()
        {
            EntityDefinition = new SettingsEntityDefinition();
            PermissionType = new PermissionType("SEOUPD", "Update SEO", "Update SEO Settings");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
