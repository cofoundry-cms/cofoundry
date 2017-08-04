using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A simple IPageBlockDisplayModelMapper that maps TInput to TOutput using AutoMapper
    /// </summary>
    /// <typeparam name="TInput">DataModel type to map from</typeparam>
    /// <typeparam name="TOutput">DisplayModel type to map to</typeparam>
    public class AutoMapperPageBlockDisplayModelMapper<TInput, TOutput> : IPageBlockDisplayModelMapper<TInput> 
        where TInput : IPageBlockTypeDataModel 
        where TOutput :IPageBlockTypeDisplayModel
    {
        public Task<IEnumerable<PageBlockDisplayModelMapperOutput>> MapAsync(IEnumerable<PageBlockDisplayModelMapperInput<TInput>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            var results = new List<PageBlockDisplayModelMapperOutput>();

            foreach (var input in inputs)
            {
                var output = Mapper.Map<TOutput>(input.DataModel);
                results.Add(input.CreateOutput(output));
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }
}
