using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Reflection;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to decorate image properties in module data providers to give properties about the image for filtering when browsing.
    /// I.e. you can specify dimensions and tags for filtering the list of images.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageAttribute : RegularExpressionAttribute, IMetadataAttribute, IEntityRelationAttribute
    {
        #region constructors

        public ImageAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAttribute"/> class.
        /// </summary>
        /// <param name="tags">An array of tags for which to filter when browsing for this image</param>
        public ImageAttribute(params string[] tags)
            : base(@"^[1-9]\d*$")
        {
            ErrorMessage = "The {0} field is required";
            Tags = tags;
        }

        #endregion

        #region interface implementation

        public void Process(DisplayMetadata modelMetaData)
        {
            modelMetaData
                .AddAdditionalValueIfNotEmpty("Tags", Tags)
                .AddAdditionalValueIfNotEmpty("Width", Width)
                .AddAdditionalValueIfNotEmpty("Height", Height)
                .AddAdditionalValueIfNotEmpty("MinWidth", MinWidth)
                .AddAdditionalValueIfNotEmpty("MinHeight", MinHeight)
                .AddAdditionalValueIfNotEmpty("PreviewWidth", PreviewWidth)
                .AddAdditionalValueIfNotEmpty("PreviewHeight", PreviewHeight)
                ;

            modelMetaData.TemplateHint = "ImageAsset";
        }

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var isRequired = !(model is int?);
            var id = (int?)propertyInfo.GetValue(model);

            if (id.HasValue)
            {
                yield return new EntityDependency(ImageAssetEntityDefinition.DefinitionCode, id.Value, isRequired);
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// The width of the image for which to browse.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the image for which to browse.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The minimum width of the image for which to browse.
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// The width to set the image at when previewing the asset. Useful if
        /// you want to preview a cropping ratio once the asset is selected.
        /// </summary>
        public int PreviewWidth { get; set; }

        /// <summary>
        /// The height to set the image at when previewing the asset. Useful if
        /// you want to preview a cropping ratio once the asset is selected.
        /// </summary>
        public int PreviewHeight { get; set; }

        /// <summary>
        /// The minimum height of the image for which to browse.
        /// </summary>
        public int MinHeight { get; set; }

        /// <summary>
        /// Filters the image search to show items with tags that match this value
        /// </summary>
        public string[] Tags { get; private set; }

        #endregion
    }
}