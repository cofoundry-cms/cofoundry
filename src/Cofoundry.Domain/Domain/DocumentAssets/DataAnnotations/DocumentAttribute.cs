using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Reflection;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use with an (optionally nullable) integer to indicate this is for the id of 
    /// a DocumentAsset. A non-null integer indicates this is a required field. Optional
    /// parameters allow you to restricts the file extensions permitted to be selected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DocumentAttribute : Attribute, IMetadataAttribute
    {
        #region constructors

        public DocumentAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAttribute"/> class.
        /// </summary>
        /// <param name="tags">An array of tags for which to filter when browsing for this document</param>
        public DocumentAttribute(params string[] tags)
        {
            Tags = tags ?? new string[0];
        }

        #endregion

        #region interface implementation

        public void Process(DisplayMetadata modelMetaData)
        {
            modelMetaData.AddAdditionalValueIfNotEmpty("Tags", Tags);
            if (FileExtensions != null && FileExtensions.Length == 1)
            {
                modelMetaData.AddAdditionalValueIfNotEmpty("FileExtension", FileExtensions.First());
            }
            else if (FileExtensions != null)
            {
                modelMetaData.AddAdditionalValueIfNotEmpty("FileExtensions", string.Join(", ", FileExtensions));
            }
            modelMetaData.TemplateHint = "DocumentAsset";
        }

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var isRequired = !(model is int?);
            var id = (int?)propertyInfo.GetValue(model);

            if (id.HasValue)
            {
                yield return new EntityDependency(DocumentAssetEntityDefinition.DefinitionCode, id.Value, isRequired);
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// Restrict the input to allow only documents with these file extensions
        /// </summary>
        public string[] FileExtensions { get; set; }

        /// <summary>
        /// Restrict the input to allow documents with only these tags
        /// </summary>
        public string[] Tags { get; private set; }

        #endregion
    }
}