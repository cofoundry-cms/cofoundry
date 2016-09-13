using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    public class CustomEntityDetailsPageViewModel<TModel> : IPageWithMetaDataViewModel, IEditablePageViewModel, IPageRoutableViewModel where TModel : ICustomEntityDetailsDisplayViewModel
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

        public PageMetaData MetaData
        {
            get
            {
                if (IsCustomModelNull()) return null;
                return CustomEntity.Model.MetaData;
            }
            set
            {
                SetCustomModelPropertyNullCheck("MetaData");
                CustomEntity.Model.MetaData = value;
            }
        }

        public PageRenderDetails Page { get; set; }

        public CustomEntityRenderDetailsViewModel<TModel> CustomEntity { get; set; }

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