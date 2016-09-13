using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain
{
    public interface IPageVersionModuleModelMapper
    {
        List<PageModuleDisplayModelMapperOutput> MapDisplayModel(string typeName, IEnumerable<IEntityVersionPageModule> entityModules, WorkFlowStatusQuery workflowStatus);
        IPageModuleDisplayModel MapDisplayModel(string typeName, IEntityVersionPageModule entityModules, WorkFlowStatusQuery workflowStatus);
        IPageModuleDataModel MapDataModel(string typeName, IEntityVersionPageModule entityModules);
    }
}
