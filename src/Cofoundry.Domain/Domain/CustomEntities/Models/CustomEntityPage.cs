using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This projection that is typically used as part of an aggregate rather
    /// than by itself and contains page region and block data for a custom
    /// entity details page.
    /// </summary>
    public class CustomEntityPage
    {
        /// <summary>
        /// Information about the page this instance is associated with.
        /// </summary>
        public PageRoute PageRoute { get; set; }

        /// <summary>
        /// The full path of the page including directories and the locale. 
        /// </summary>
        public string FullUrlPath { get; set; }

        /// <summary>
        /// All region and block data for this custom entity page.
        /// </summary>
        public ICollection<CustomEntityPageRegionDetails> Regions { get; set; }
    }
}
