using Conditions;
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
    /// Use this to decorate an integer array and indicate that it should be a collection of custom entity ids for a specific 
    /// custom entity type. Optional parameters indicate whether the collection is sortable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomEntityCollectionAttribute : Attribute, IMetadataAttribute, IEntityRelationAttribute
    {
        public CustomEntityCollectionAttribute(string customEntityDefinitionCode)
            : base()
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        public void Process(DisplayMetadata modelMetaData)
        {
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
        /// Can the collection be manually ordered by the user?
        /// </summary>
        public bool IsOrderable { get; set; }
        
        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            Condition.Requires(model).IsNotNull();
            Condition.Requires(propertyInfo).IsNotNull();

            var ids = propertyInfo.GetValue(model) as int[];

            if (ids != null)
            {
                foreach (var id in ids)
                {
                    yield return new EntityDependency(CustomEntityDefinitionCode, id, false);
                }
            }
        }
    }
}
