using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
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

        public bool ForceUrlSlugUniqueness { get; set; }
    }
}
