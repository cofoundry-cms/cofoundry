using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A simple IPageModuleDisplayModelMapper that maps TInput to TOutput using AutoMapper
    /// </summary>
    /// <typeparam name="TInput">DataModel type to map from</typeparam>
    /// <typeparam name="TOutput">DisplayModel type to map to</typeparam>
    public class AutoMapperPageModuleDisplayModelMapper<TInput, TOutput> : IPageModuleDisplayModelMapper<TInput> 
        where TInput : IPageModuleDataModel 
        where TOutput :IPageModuleDisplayModel
    {
        public IEnumerable<PageModuleDisplayModelMapperOutput> Map(IEnumerable<PageModuleDisplayModelMapperInput<TInput>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            foreach (var input in inputs)
            {
                var output = Mapper.Map<TOutput>(input.DataModel);
                yield return input.CreateOutput(output);
            }
        }
    }
}
