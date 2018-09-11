using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the result of a block data mapping operation for a single
    /// page block type.
    /// </summary>
    /// <typeparam name="TDataModel">The data model type being mapped from.</typeparam>
    public class PageBlockTypeDisplayModelMapperResult<TDataModel>
        where TDataModel : IPageBlockTypeDataModel
    {
        private readonly Dictionary<int, IPageBlockTypeDisplayModel> _mappedDisplayModels;

        /// <summary>
        /// Represents the result of a block data mapping operation for a single
        /// page block type.
        /// </summary>
        /// <param name="inputItemCount">
        /// The input data model collection size, which is used to set the
        /// initial size of the result collection.
        /// </param>
        public PageBlockTypeDisplayModelMapperResult(int inputItemCount)
        {
            _mappedDisplayModels = new Dictionary<int, IPageBlockTypeDisplayModel>(inputItemCount);
        }

        /// <summary>
        /// Collection of mapped models used internally once mapping is complete. Not
        /// really expected to be used during mapping.
        /// </summary>
        public IReadOnlyDictionary<int, IPageBlockTypeDisplayModel> Items { get { return _mappedDisplayModels; } }

        /// <summary>
        /// Adds a mapped model to the result collection.
        /// </summary>
        /// <param name="inputDataModel">
        /// The input data model that the display model is mapped from. This is 
        /// used to track which block the mapped display model represents.
        /// </param>
        /// <param name="mappedDisplayModel">
        /// The mapped display model to render in the view. This should not
        /// be null; if the input cannot be mapped then exclude it from the 
        /// result entirely.
        /// </param>
        public void Add(PageBlockTypeDisplayModelMapperInput<TDataModel> inputDataModel, IPageBlockTypeDisplayModel mappedDisplayModel)
        {
            if (inputDataModel == null) throw new ArgumentNullException(nameof(inputDataModel));
            if (mappedDisplayModel == null) throw new ArgumentNullException(nameof(mappedDisplayModel), "When mapping block data the display model should not be null, if the input cannot be mapped then exclude it from the result entirely.");

            if (_mappedDisplayModels.ContainsKey(inputDataModel.VersionBlockId))
            {
                throw new Exception($"The specified block model has already been added to the result. VersionBlockId {inputDataModel.VersionBlockId}, model type '{mappedDisplayModel.GetType()}'");
            }

            _mappedDisplayModels.Add(inputDataModel.VersionBlockId, mappedDisplayModel);
        }
    }
}
