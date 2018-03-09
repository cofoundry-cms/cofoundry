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
    /// Use this to decorate an collection of datamodel. Optional parameters indicate whether the collection is sortable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NestedDataModelCollectionAttribute : Attribute, IMetadataAttribute, IEntityRelationAttribute
    {
        /// <summary>
        /// The minimum number of items that need to be included in the collection. 0 indicates
        /// no minimum.
        /// </summary>
        public int MinItems { get; set; }

        /// <summary>
        /// The maximum number of items that can be included in the collection. 0 indicates
        /// no maximum.
        /// </summary>
        public int MaxItems { get; set; }

        /// <summary>
        /// Can the collection be manually ordered by the user?
        /// </summary>
        public bool IsOrderable { get; set; }

        public void Process(DisplayMetadataProviderContext context)
        {
            string nestedModelTypeName = GetNestedModelTypeName(context);
            var modelMetaData = context.DisplayMetadata;

            modelMetaData
                .AddAdditionalValueIfNotEmpty("MinItems", MinItems)
                .AddAdditionalValueIfNotEmpty("MaxItems", MaxItems)
                .AddAdditionalValueIfNotEmpty("Orderable", IsOrderable)
                .AddAdditionalValueIfNotEmpty("ModelType", nestedModelTypeName);

            modelMetaData.TemplateHint = "NestedDataModelCollection";
        }

        private string GetNestedModelTypeName(DisplayMetadataProviderContext context)
        {
            var propertyModelType = context.Key.ModelType;

            if (!propertyModelType.IsGenericType)
            {
                throw new Exception(GetBadTypeExceptionMessage());
            }

            var genericParameters = propertyModelType.GetGenericArguments();
            if (genericParameters?.Length != 1 || !typeof(INestedDataModel).IsAssignableFrom(genericParameters.Single()))
            {
                throw new Exception(GetBadTypeExceptionMessage());
            }

            var nestedModelName = StringHelper.RemoveSuffix(genericParameters.Single().Name, "DataModel", StringComparison.OrdinalIgnoreCase);
            return nestedModelName;
        }

        private string GetBadTypeExceptionMessage()
        {
            return $"{typeof(NestedDataModelCollectionAttribute).Name} can only be placed on properties with a generic collection of models that inherit from {typeof(INestedDataModel).Name}";
        }

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var nestedItems = propertyInfo.GetValue(model) as IEnumerable<INestedDataModel>;

            var dependencies = EnumerableHelper
                .Enumerate(nestedItems)
                .SelectMany(EntityRelationAttributeHelper.GetRelations);

            return dependencies;
        }
    }
}
