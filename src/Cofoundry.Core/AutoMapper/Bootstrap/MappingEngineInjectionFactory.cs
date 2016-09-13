using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Core.AutoMapper
{
    public class MappingEngineInjectionFactory : IInjectionFactory<IMapper>
    {
        public IMapper Create()
        {
            return Mapper.Instance;
        }
    }
}
