using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate an integer collection and indicate that it should be a 
    /// collection of custom entity ids for a specific custom entity type. Optional
    /// parameters indicate whether the collection is sortable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomEntityCollectionAttribute : Attribute, IMetadataAttribute, IEntityRelationAttribute
    {
        public CustomEntityCollectionAttribute(string customEntityDefinitionCode)
            : base()
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        public void Process(DisplayMetadataProviderContext context)
        {
            MetaDataAttributePlacementValidator.ValidateCollectionPropertyType(this, context, typeof(int));

            var modelMetaData = context.DisplayMetadata;

            modelMetaData
                .AddAdditionalValueIfNotEmpty("CustomEntityDefinitionCode", CustomEntityDefinitionCode)
                .AddAdditionalValueIfNotEmpty("Orderable", IsOrderable);

            modelMetaData.TemplateHint = "CustomEntityCollection";
        }

        /// <summary>
        /// The code of the custom entity which is allowed to be attached to the collection.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Set to true to allow the collection ordering to be set by an editor 
        /// using a drag and drop interface. Defaults to false.
        /// </summary>
        public bool IsOrderable { get; set; }
        
        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var ids = propertyInfo.GetValue(model) as ICollection<int>;

            foreach (var id in EnumerableHelper.Enumerate(ids))
            {
                yield return new EntityDependency(CustomEntityDefinitionCode, id, false);
            }
        }
    }
}
