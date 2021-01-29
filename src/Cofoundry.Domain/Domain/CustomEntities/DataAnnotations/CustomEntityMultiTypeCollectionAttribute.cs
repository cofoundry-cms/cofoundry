using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate a collection of `CustomEntityIdentity` objects, indicating 
    /// the property represents a set of custom entities of mixed types. The entity types 
    /// must be defined in the attribute constructor by passing in custom entity definition 
    /// codes. Optional parameters indicate whether the collection is sortable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomEntityMultiTypeCollectionAttribute : Attribute, IMetadataAttribute, IEntityRelationAttribute
    {
        public CustomEntityMultiTypeCollectionAttribute(params string[] customEntityDefinitionCode)
            : base()
        {
            CustomEntityDefinitionCodes = customEntityDefinitionCode;
        }

        public void Process(DisplayMetadataProviderContext context)
        {
            MetaDataAttributePlacementValidator.ValidateCollectionPropertyType(this, context, typeof(CustomEntityIdentity));

            var modelMetaData = context.DisplayMetadata;

            modelMetaData
                .AddAdditionalValueIfNotEmpty("CustomEntityDefinitionCodes", string.Join(",", CustomEntityDefinitionCodes))
                .AddAdditionalValueIfNotEmpty("Orderable", IsOrderable)
                .AddAdditionalValueIfNotNull("TitleColumnHeader", TitleColumnHeader)
                .AddAdditionalValueIfNotNull("DescriptionColumnHeader", DescriptionColumnHeader)
                .AddAdditionalValueIfNotNull("ImageColumnHeader", ImageColumnHeader)
                .AddAdditionalValueIfNotNull("TypeColumnHeader", TypeColumnHeader);

            modelMetaData.TemplateHint = "CustomEntityMultiTypeCollection";
        }

        /// <summary>
        /// The code of the custom entity which is allowed to be attached to the collection.
        /// </summary>
        public string[] CustomEntityDefinitionCodes { get; set; }

        /// <summary>
        /// Set to true to allow the collection ordering to be set by an editor using a drag 
        /// and drop interface. Defaults to false.
        /// </summary>
        public bool IsOrderable { get; set; }

        /// <summary>
        /// The text to use in the column header for the title field. Defaults
        /// to "Title".
        /// </summary>
        public string TitleColumnHeader { get; set; }

        /// <summary>
        /// The text to use in the column header for the description field. Defaults
        /// to "Description".
        /// </summary>
        public string DescriptionColumnHeader { get; set; }

        /// <summary>
        /// The text to use in the column header for the image field. Defaults
        /// to empty string.
        /// </summary>
        public string ImageColumnHeader { get; set; }

        /// <summary>
        /// The text to use in the column header for the model type field. Defaults
        /// to "Type".
        /// </summary>
        public string TypeColumnHeader { get; set; }

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var ids = propertyInfo.GetValue(model) as ICollection<CustomEntityIdentity>;

            foreach (var id in EnumerableHelper.Enumerate(ids))
            {
                yield return new EntityDependency(id.CustomEntityDefinitionCode, id.CustomEntityId, false);
            }
        }
    }
}
