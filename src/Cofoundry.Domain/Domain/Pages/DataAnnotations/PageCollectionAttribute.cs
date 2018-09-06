using System;
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
    /// Use this to decorate an integer array and indicate that it should be a
    /// collection of database ids for Pages. Optional parameters indicate 
    /// whether the collection is sortable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PageCollectionAttribute : Attribute, IMetadataAttribute, IEntityRelationAttribute
    {
        public void Process(DisplayMetadataProviderContext context)
        {
            MetaDataAttributePlacementValidator.ValidateCollectionPropertyType(this, context, typeof(int));

            var modelMetaData = context.DisplayMetadata;

            modelMetaData.AddAdditionalValueIfNotEmpty("Orderable", IsOrderable);
            modelMetaData.TemplateHint = "PageCollection";
        }

        /// <summary>
        /// Can the collection be manually ordered by the user?
        /// </summary>
        public bool IsOrderable { get; set; }
        
        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var ids = propertyInfo.GetValue(model) as ICollection<int>;
            
            foreach (var id in EnumerableHelper.Enumerate(ids))
            {
                yield return new EntityDependency(PageEntityDefinition.DefinitionCode, id, false);
            }
        }
    }
}
