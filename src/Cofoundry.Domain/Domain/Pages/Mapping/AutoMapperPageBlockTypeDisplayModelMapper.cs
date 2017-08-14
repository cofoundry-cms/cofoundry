using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A simple IPageBlockTypeDisplayModelMapper that maps TInput to TOutput using AutoMapper
    /// </summary>
    /// <typeparam name="TInput">DataModel type to map from</typeparam>
    /// <typeparam name="TOutput">DisplayModel type to map to</typeparam>
    public class AutoMapperPageBlockTypeDisplayModelMapper<TInput, TOutput> : IPageBlockTypeDisplayModelMapper<TInput> 
        where TInput : IPageBlockTypeDataModel 
        where TOutput :IPageBlockTypeDisplayModel
    {
        public Task<IEnumerable<PageBlockTypeDisplayModelMapperOutput>> MapAsync(IEnumerable<PageBlockTypeDisplayModelMapperInput<TInput>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            var results = new List<PageBlockTypeDisplayModelMapperOutput>();

            foreach (var input in inputs)
            {
                var output = Mapper.Map<TOutput>(input.DataModel);
                results.Add(input.CreateOutput(output));
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }
}
