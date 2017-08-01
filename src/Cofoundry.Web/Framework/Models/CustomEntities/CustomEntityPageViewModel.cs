using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// A page view model class for a custom entity page that includes detailed custom
    /// entity data. This is the default ICustomEntityPageViewModel implementation, but you
    /// can override this implementation with your own by implementing a custom IPageViewModelFactory.
    /// </summary>
    /// <typeparam name="TDisplayModel">The type of view model used to represent the custom entity data model when formatted for display.</typeparam>
    public class CustomEntityPageViewModel<TDisplayModel> : ICustomEntityPageViewModel<TDisplayModel>
        where TDisplayModel : ICustomEntityPageDisplayModel
    {
        public string PageTitle
        {
            get
            {
                if (IsCustomModelNull()) return null;
                return CustomEntity.Model.PageTitle;
            }
            set
            {
                SetCustomModelPropertyNullCheck("PageTitle");
                CustomEntity.Model.PageTitle = value;
            }
        }

        public string MetaDescription
        {
            get
            {
                if (IsCustomModelNull()) return null;
                return CustomEntity.Model.MetaDescription;
            }
            set
            {
                SetCustomModelPropertyNullCheck("MetaDescription");
                CustomEntity.Model.MetaDescription = value;
            }
        }

        /// <summary>
        /// Data about the page this custom entity instance is hosted in. For custom
        /// entity pages the page entity forms a template for each custom entity
        /// </summary>
        public PageRenderDetails Page { get; set; }

        public CustomEntityRenderDetailsViewModel<TDisplayModel> CustomEntity { get; set; }

        public bool IsPageEditMode { get; set; }

        public PageRoutingHelper PageRoutingHelper { get; set; }

        #region helpers

        private bool IsCustomModelNull()
        {
            return CustomEntity == null || CustomEntity.Model == null;
        }

        private void SetCustomModelPropertyNullCheck(string property)
        {
            if (IsCustomModelNull())
            {
                throw new NullReferenceException("Cannot set the " + property + " property, the Page property has not been set.");
            }
        }

        #endregion
    }
}