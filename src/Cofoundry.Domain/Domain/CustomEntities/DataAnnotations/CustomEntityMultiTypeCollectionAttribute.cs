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
    /// Use this to decorate a collection of `CustomEntityIdentity` types and indicate that it should be a 
    /// collection of custom entities for a mix of custom entity types. Optional parameters indicate 
    /// whether the collection is sortable.
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
                .AddAdditionalValueIfNotEmpty("Orderable", IsOrderable);

            modelMetaData.TemplateHint = "CustomEntityMultiTypeCollection";
        }

        /// <summary>
        /// The code of the custom entity which is allowed to be attached to the collection.
        /// </summary>
        public string[] CustomEntityDefinitionCodes { get; set; }

        /// <summary>
        /// Can the collection be manually ordered by the user?
        /// </summary>
        public bool IsOrderable { get; set; }
        
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
