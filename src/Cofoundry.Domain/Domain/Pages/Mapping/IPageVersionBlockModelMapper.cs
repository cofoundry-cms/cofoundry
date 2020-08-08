using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Helper for mapping page and custom entity version block data from an 
    /// unstructured db source to a display model instance.
    /// </summary>
    public interface IPageVersionBlockModelMapper
    {
        /// <summary>
        /// Maps a batch of the same type of page block data to a collection
        /// of display models ready for rendering.
        /// </summary>
        /// <param name="typeName">The block type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="entityBlocks">The version data to get the serialized model from.</param>
        /// <param name="publishStatus">
        /// The publish status of the parent page or custom entity 
        /// being mapped. This is provided so dependent entities can use
        /// the same publish status.
        /// </param>
        /// <param name="executionContext">
        /// The execution context from the caller which can be used in
        /// any child queries to ensure any elevated permissions are 
        /// passed down the chain of execution.
        /// </param>
        /// <returns>
        /// Dictionary of mapped display models, with a key (block version id) that can be 
        /// used to identify them.
        /// </returns>
        Task<IReadOnlyDictionary<int, IPageBlockTypeDisplayModel>> MapDisplayModelAsync(string typeName, IEnumerable<IEntityVersionPageBlock> entityBlocks, PublishStatusQuery publishStatus, IExecutionContext executionContext);

        /// <summary>
        /// Maps a single page block data model to a concrete display model.
        /// </summary>
        /// <param name="typeName">The block type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="entityBlock">The version data to get the serialized model from.</param>
        /// <param name="publishStatus">
        /// The publish status of the parent page or custom entity 
        /// being mapped. This is provided so dependent entities can use
        /// the same publish status.
        /// </param>
        /// <param name="executionContext">
        /// The execution context from the caller which can be used in
        /// any child queries to ensure any elevated permissions are 
        /// passed down the chain of execution.
        /// </param>
        /// <returns>Mapped display model.</returns>
        Task<IPageBlockTypeDisplayModel> MapDisplayModelAsync(string typeName, IEntityVersionPageBlock entityBlock, PublishStatusQuery publishStatus, IExecutionContext executionContext);

        /// <summary>
        /// Deserialized a block data model to a stongly typed model.
        /// </summary>
        /// <param name="typeName">The block type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="entityPageBlocks">The data to get the serialized model from.</param>
        /// <returns>Strongly typed data model including deserialized data.</returns>
        IPageBlockTypeDataModel MapDataModel(string typeName, IEntityVersionPageBlock entityPageBlocks);
    }
}
