using Cofoundry.Core;
using Cofoundry.Core.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate a collection of NestedDataModelMultiTypeItem
    /// objects, indicating the property represents a set of nested data 
    /// models of mixed types. The types must be defined in the attribute 
    /// constructor by passing in a type reference for each nested data model 
    /// you want to be able to add.
    /// is sortable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NestedDataModelMultiTypeCollectionAttribute : ValidateObjectAttribute, IMetadataAttribute, IEntityRelationAttribute
    {
        private readonly IEnumerable<Type> _types;

        /// <summary>
        /// Use this to decorate a collection of NestedDataModelMultiTypeItem objects, allowing them 
        /// to be edited in the admin UI. Optional parameters indicate whether the collection 
        /// is sortable.
        /// </summary>
        /// <param name="types">
        /// The data model types that are permitted to be added to the collection. Each type
        /// must implement the INestedDataModel  market interface.
        /// </param>
        public NestedDataModelMultiTypeCollectionAttribute(Type[] types)
        {
            _types = types;
        }

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

        public void Process(DisplayMetadataProviderContext context)
        {
            ValidateModelType(context);

            var nestedModelTypeNames = _types
                .Select(t => t.Name)
                .ToList();

            var modelMetaData = context.DisplayMetadata;

            modelMetaData
                .AddAdditionalValueIfNotEmpty("MinItems", MinItems)
                .AddAdditionalValueIfNotEmpty("MaxItems", MaxItems)
                .AddAdditionalValueIfNotEmpty("Orderable", IsOrderable)
                .AddAdditionalValueIfNotNull("TitleColumnHeader", TitleColumnHeader)
                .AddAdditionalValueIfNotNull("DescriptionColumnHeader", DescriptionColumnHeader)
                .AddAdditionalValueIfNotNull("ImageColumnHeader", ImageColumnHeader)
                .AddAdditionalValueIfNotNull("TypeColumnHeader", TypeColumnHeader)
                .AddAdditionalValueIfNotEmpty("ModelTypes", nestedModelTypeNames);

            modelMetaData.TemplateHint = "NestedDataModelMultiTypeCollection";
        }

        /// <summary>
        /// Validates that the property type is an enumerable collection
        /// of INestedDataModel types e.g. ICollection<INestedDataModel>.
        /// It's less likely but also possible that the type could be an 
        /// interface or base class that inherits from INestedDataModel e.g.
        /// you may want all your nested model types to implement a common 
        /// interface e.g. IContentBlockDataModel.
        /// </summary>
        private void ValidateModelType(DisplayMetadataProviderContext context)
        {
            var singularType = TypeHelper.GetCollectionTypeOrNull(context.Key.ModelType);

            if (singularType == null)
            {
                throw GetIncorrectTypeException(context);
            }

            if (!typeof(NestedDataModelMultiTypeItem).IsAssignableFrom(singularType))
            {
                throw GetIncorrectTypeException(context);
            }
        }

        private IncorrectCollectionMetaDataAttributePlacementException GetIncorrectTypeException(DisplayMetadataProviderContext context)
        {
            var propertyName = context.Key.ContainerType.Name + "." + context.Key.Name;
            var msg = $"{nameof(NestedDataModelMultiTypeCollectionAttribute)} can only be placed on properties with a generic collection of {typeof(NestedDataModelMultiTypeItem).Name} types. Property name is {propertyName} and the type is {context.Key.ModelType}.";
            var exception = new IncorrectCollectionMetaDataAttributePlacementException(this, context, new Type[] { typeof(NestedDataModelMultiTypeItem) }, msg);

            return exception;
        }

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var nestedItems = propertyInfo.GetValue(model) as IEnumerable<NestedDataModelMultiTypeItem>;

            var dependencies = EnumerableHelper
                .Enumerate(nestedItems)
                .Select(i => i.Model)
                .SelectMany(EntityRelationAttributeHelper.GetRelations);

            return dependencies;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var collection = value as IEnumerable<NestedDataModelMultiTypeItem>;

            if (MinItems > 0 && EnumerableHelper.Enumerate(collection).Count() < MinItems)
            {
                return new ValidationResult(validationContext.MemberName + $" must have at least {MinItems} items.", new string[] { validationContext.MemberName });
            }

            if (MaxItems > 0 && EnumerableHelper.Enumerate(collection).Count() > MaxItems)
            {
                return new ValidationResult(validationContext.MemberName + $" cannot have more than {MaxItems} items.", new string[] { validationContext.MemberName });
            }

            return base.IsValid(value, validationContext);
        }
    }
}
