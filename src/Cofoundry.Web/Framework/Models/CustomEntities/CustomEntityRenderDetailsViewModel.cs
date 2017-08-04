using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Custom entity view model data used in a ICustomEntityPageViewModel implementation. This is 
    /// similar to CustomEntityRenderDetails from the domain, but adds a typed display model that 
    /// is mapped from the raw custom entity data model.
    /// </summary>
    /// <typeparam name="TDisplayModel">The type of view model used to represent the custom entity data model when formatted for display.</typeparam>
    public class CustomEntityRenderDetailsViewModel<TDisplayModel>
    {
        public int CustomEntityId { get; set; }

        public int CustomEntityVersionId { get; set; }

        public ActiveLocale Locale { get; set; }

        public string Title { get; set; }

        public string UrlSlug { get; set; }

        public TDisplayModel Model { get; set; }

        public IEnumerable<CustomEntityPageRegionRenderDetails> Regions { get; set; }

        public WorkFlowStatus WorkFlowStatus { get; set; }
    }
}
