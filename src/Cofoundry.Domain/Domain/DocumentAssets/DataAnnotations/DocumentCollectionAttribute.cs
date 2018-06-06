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
    /// Use this to decorate an integer collection of DocumentAssetIds and indicate that it should be a collection 
    /// of document assets. The editor allows for sorting of linked document assets and you can set filters for restricting file types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DocumentCollectionAttribute : Attribute, IMetadataAttribute
    {
        #region constructor

        public DocumentCollectionAttribute()
            : base()
        {
        }

        #endregion

        #region interface implementation

        public void Process(DisplayMetadataProviderContext context)
        {
            var modelMetaData = context.DisplayMetadata;

            DocumentAttributeMetaDataHelper.AddFilterData(modelMetaData, FileExtensions, Tags);

            modelMetaData.TemplateHint = "DocumentAssetCollection";
        }

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var ids = propertyInfo.GetValue(model) as ICollection<int>;

            foreach (var id in EnumerableHelper.Enumerate(ids))
            {
                yield return new EntityDependency(DocumentAssetEntityDefinition.DefinitionCode, id, false);
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
        public string[] Tags { get; set; }

        #endregion
    }
}
