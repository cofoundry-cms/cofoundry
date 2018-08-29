using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class PageViewModel : IPageViewModel
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
                SetPagePropertyNullCheck(nameof(PageTitle));
                Page.Title = value;
            }
        }

        public string MetaDescription
        { 
            get
            {
                if (Page == null) return null;
                return Page.MetaDescription;
            }
            set
            {
                SetPagePropertyNullCheck(nameof(MetaDescription));
                Page.MetaDescription = value;
            } 
        }

        private void SetPagePropertyNullCheck(string property)
        {
            if (Page == null)
            {
                throw new NullReferenceException($"Cannot set the {property} property, the Page property has not been set.");
            }
        }

        public PageRoutingHelper PageRoutingHelper { get; set; }
    }
}