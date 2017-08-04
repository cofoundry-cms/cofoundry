using Cofoundry.Core.AutoUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Installation
{
    public class RegisterPageTemplatesAndPageBlockTypesCommand : IAlwaysRunUpdateCommand
    {
        public string Description
        {
            get
            {
                return "Update page templates, page block types & block type templates";
            }
        }
    }
}
