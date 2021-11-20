using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// Data model classes can use data annotations to describe the data
    /// and provide hints to the admin UI as to how the property should be 
    /// displayed.
    /// 
    /// In this data model we link out to images and other custom entities.
    /// Although the data model serialized as json in the database, the
    /// relationships are stored separately which allows us to provide a certain
    /// amount of data integrity.
    /// </summary>
    public class CatDataModel : ICustomEntityDataModel
    {
        [Display(Description = "A short description or tag-line to describe the cat")]
        public string Description { get; set; }

        [Display(Name = "Breed", Description = "Identity the breed of cat if possible")]
        [CustomEntity(BreedCustomEntityDefinition.DefinitionCode)]
        public int? BreedId { get; set; }

        [Display(Name = "Features", Description = "Extra features or properties that help categorize this cat")]
        [CustomEntityCollection(FeatureCustomEntityDefinition.DefinitionCode)]
        public ICollection<int> FeatureIds { get; set; }

        [Display(Name = "Images", Description = "The top image will be the main image that displays in the grid")]
        [ImageCollection]
        public ICollection<int> ImageAssetIds { get; set; }
    }
}
