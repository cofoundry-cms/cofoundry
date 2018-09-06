using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This model is a lightweight projection of the data defined in a custom entity 
    /// definition class. This is typically used as part of another domain model or
    /// for querying lists of definitions in the admin panel.
    /// </summary>
    public class CustomEntityDefinitionMicroSummary
    {
        /// <summary>
        /// Unique 6 letter code representing the entity (use uppercase)
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Plural name of the entity e.g. 'Products'
        /// </summary>
        public string NamePlural { get; set; }

        /// <summary>
        /// Singlar name of the entity e.g. 'Product'
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A short description that shows up as a tooltip for the admin 
        /// module. E.g  "Products and stock." or "News items for shareholders"
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates whether the UrlSlug property should be treated
        /// as a unique property and be validated as such. This will also affect
        /// the routing templates available for this entity because some routes require
        /// a unique slug.
        /// </summary>
        public bool ForceUrlSlugUniqueness { get; set; }
    }
}
