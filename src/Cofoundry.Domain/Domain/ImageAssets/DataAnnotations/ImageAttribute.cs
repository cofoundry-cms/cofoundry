using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Reflection;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to decorate image properties in dynamic data providers to give properties about the image for filtering when browsing.
    /// I.e. you can specify dimensions and tags for filtering the list of images.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageAttribute : RegularExpressionAttribute, IMetadataAttribute, IEntityRelationAttribute
    {
        #region constructors

        /// <summary>
        /// This can be used to decorate image properties in dynamic data providers to give properties about the image for filtering when browsing.
        /// I.e. you can specify dimensions and tags for filtering the list of images.
        /// </summary>
        public ImageAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// This can be used to decorate image properties in dynamic data providers to give properties about the image for filtering when browsing.
        /// I.e. you can specify dimensions and tags for filtering the list of images.
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

        public void Process(DisplayMetadataProviderContext context)
        {
            MetaDataAttributePlacementValidator.ValidatePropertyType(this, context, typeof(int), typeof(int?));

            var modelMetaData = context.DisplayMetadata;

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
        /// Filters the image selection to only show items with a width exactly
        /// the same as this value.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Filters the image selection to only show items with a height exactly
        /// the same as this value.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Filters the image selection to only show items with a width the same or
        /// greater than this value.
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// Filters the image selection to only show items with a height  the same or
        /// greater than this value.
        /// </summary>
        public int MinHeight { get; set; }

        /// <summary>
        /// The width to use when previewing the image in 
        /// the admin panel. This is useful if you want to preview the 
        /// image in the selector at a specific crop ratio, e.g.
        /// a letterbox ratio for a banner.
        /// </summary>
        public int PreviewWidth { get; set; }

        /// <summary>
        /// The height to use when when previewing the image in 
        /// the admin panel. This is useful if you want to preview the 
        /// image in the selector at a specific crop ratio, e.g.
        /// a letterbox ratio for a banner.
        /// </summary>
        public int PreviewHeight { get; set; }

        /// <summary>
        /// Filters the image selection to only show images with tags that 
        /// match this value.
        /// </summary>
        public string[] Tags { get; set; }

        #endregion
    }
}