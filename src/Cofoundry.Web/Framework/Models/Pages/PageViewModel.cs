using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class PageViewModel : IPageWithMetaDataViewModel, IEditablePageViewModel, IPageRoutableViewModel
    {
        public PageRenderDetails Page { get; set; }
        
        public bool IsPageEditMode { get; set; }

        public string PageTitle
        {
            get
            {
                if (Page == null) return null;
                return Page.Title;
            }
            set
            {
                SetPagePropertyNullCheck("PageTitle");
                Page.Title = value;
            }
        }

        public PageMetaData MetaData 
        { 
            get
            {
                if (Page == null) return null;
                return Page.MetaData;
            }
            set
            {
                SetPagePropertyNullCheck("MetaData");
                Page.MetaData = value;
            } 
        }

        private void SetPagePropertyNullCheck(string property)
        {
            if (Page == null)
            {
                throw new NullReferenceException("Cannot set the " + property  + " property, the Page property has not been set.");
            }
        }

        public PageRoutingHelper PageRoutingHelper { get; set; }
    }
}