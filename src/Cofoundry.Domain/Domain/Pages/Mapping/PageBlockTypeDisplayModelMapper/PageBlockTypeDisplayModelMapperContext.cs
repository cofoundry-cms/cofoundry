using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Acts as a container for contextual data used in a an 
    /// implementation of IPageBlockTypeDisplayModelMapper, as
    /// well as the collection of data model items to be mapped.
    /// </summary>
    /// <typeparam name="TDataModel">The data model type being mapped.</typeparam>
    public class PageBlockTypeDisplayModelMapperContext<TDataModel>
        where TDataModel : IPageBlockTypeDataModel
    {
        public PageBlockTypeDisplayModelMapperContext(
             IReadOnlyCollection<PageBlockTypeDisplayModelMapperInput<TDataModel>> items,
             PublishStatusQuery publishStatusQuery,
             IExecutionContext executionContext
             )
        {
            if (items == null) throw new ArgumentNullException(nameof(Items));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            Items = items;
            PublishStatusQuery = publishStatusQuery;
            ExecutionContext = executionContext;
        }

        /// <summary>
        /// A collection of block data model items to be mapped into display models 
        /// by the mapper. In your MapAsync method interate over this collection and
        /// map each item to a display model and then add it to the result.
        /// </summary>
        public IReadOnlyCollection<PageBlockTypeDisplayModelMapperInput<TDataModel>> Items { get; private set; }

        /// <summary>
        /// A modified version of the parent page publish status suitable to be used 
        /// to query dependent entities. This isn't necessarily the same as the parent page
        /// status, E.g. PublishStatusQuery.SpecificVersion cannot be used to query a 
        /// dependent entity and so PublishStatusQuery.Latest is used instead.
        /// </summary>
        public PublishStatusQuery PublishStatusQuery { get; private set; }

        /// <summary>
        /// The execution context from the caller which can be used in
        /// any child queries to ensure any elevated permissions are 
        /// passed down the chain of execution.
        /// </summary>
        public IExecutionContext ExecutionContext { get; private set; }
    }
}
