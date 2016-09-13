using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain.Installation
{
    public class ImportPermissionsCommand : IUpdateCommand
    {
        public string Description
        {
            get { return GetType().Name; }
        }

        public int Version
        {
            get { return 1; }
        }
    }
}
