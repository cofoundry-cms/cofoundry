using Cofoundry.Core.AutoUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Installation
{
    public class RegisterPageTemplatesAndModuleTypesCommand : IAlwaysRunUpdateCommand
    {
        public string Description
        {
            get
            {
                return "Update Page Templates, Module Types & Module Type Templates";
            }
        }
    }
}
