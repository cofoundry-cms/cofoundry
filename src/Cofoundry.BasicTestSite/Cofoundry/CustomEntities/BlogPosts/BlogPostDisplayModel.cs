using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    /// <summary>
    /// An ICustomEntityPageDisplayModel implementation is required if
    /// you want to use a page template to dynamically render a details view
    /// of a custom entity. This provides us with a strongly typed model to use
    /// in the template.
    /// </summary>
    public class BlogPostDisplayModel : ICustomEntityPageDisplayModel<BlogPostDataModel>
    {
        public string PageTitle { get; set; }

        public string MetaDescription { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string FullPath { get; set; }

        public ICollection<CategorySummary> Categories { get; set; }
    }
}