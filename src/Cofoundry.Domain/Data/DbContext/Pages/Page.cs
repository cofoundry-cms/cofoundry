using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Pages represent the dynamically navigable pages of your website. Each page uses a template 
    /// which defines the regions of content that users can edit. Pages are a versioned entity and 
    /// therefore have many page version records. At one time a page may only have one draft 
    /// version, but can have many published versions; the latest published version is the one that 
    /// is rendered when the page is published. 
    /// </summary>
    /// <inheritdoc/>
    public class Page : IEntityAccessRestrictable<PageAccessRule>, ICreateAuditable, IEntityPublishable
    {
        /// <summary>
        /// The auto-incrementing database id of the page.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// The id of the <see cref="PageDirectory"/> this page is in.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// The <see cref="PageDirectory"/> this page is in.
        /// </summary>
        public virtual PageDirectory PageDirectory { get; set; }

        /// <summary>
        /// Optional id of the <see cref="Locale"/> if used in a localized site.
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// Optional <see cref="Locale"/> of the page if used in a localized site.
        /// </summary>
        public virtual Locale Locale { get; set; }

        /// <summary>
        /// The path of the page within the directory. This must be
        /// unique within the directory the page is parented to.
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// Most pages are generic pages but they could have some sort of
        /// special function e.g. NotFound, CustomEntityDetails. This is the
        /// numeric representation of the domain <see cref="Cofoundry.Domain.PageType"/> 
        /// enum.
        /// </summary>
        public int PageTypeId { get; set; }

        /// <summary>
        /// If this is of <see cref="PageType.CustomEntityDetails"/>, this is used
        /// to look up the routing.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// If this is of <see cref="PageType.CustomEntityDetails"/>, this is used
        /// to look up the routing.
        /// </summary>
        public virtual CustomEntityDefinition CustomEntityDefinition { get; set; }

        public string PublishStatusCode { get; set; }

        /// <summary>
        /// The date and time that the page is or should be published.
        /// The publish date should always be set if the <see cref="PublishStatusCode"/> 
        /// is set to "P" (Published). Generally this tracks the first or original publish 
        /// date, with subsequent publishes only updating the <see cref="LastPublishDate"/>,
        /// however the <see cref="PublishDate"/> can be set to a specific date to allow for
        /// scheduled publishing.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// The date and time that the page was last published. This can be different to
        /// <see cref="PublishDate"/> which is generally the date the page was originally
        /// published, with this property relecting any subsequent updates. The <see cref="PublishDate"/> 
        /// can be set manually to a future date when publishing, however the change is also 
        /// reflected in <see cref="LastPublishDate"/> if it is scheduled ahead of the existing 
        /// <see cref="LastPublishDate"/>.
        /// </summary>
        public DateTime? LastPublishDate { get; set; }

        public int AccessRuleViolationActionId { get; set; }

        public string UserAreaCodeForSignInRedirect { get; set; }

        public virtual UserArea UserAreaForSignInRedirect { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        public virtual User Creator { get; set; }

        public virtual ICollection<PageGroupItem> PageGroupItems { get; set; } = new List<PageGroupItem>();

        /// <summary>
        /// Tags can be used to categorize an entity.
        /// </summary>
        public virtual ICollection<PageTag> PageTags { get; set; } = new List<PageTag>();

        /// <summary>
        /// Pages are a versioned entity and therefore have many page version
        /// records. At one time a page may only have one draft version, but
        /// can have many published versions; the latest published version is
        /// the one that is rendered when the page is published. 
        /// </summary>
        public virtual ICollection<PageVersion> PageVersions { get; set; } = new List<PageVersion>();

        /// <summary>
        /// Lookup cache used for quickly finding the correct version for a
        /// specific publish status query e.g. 'Latest', 'Published', 'PreferPublished'
        /// </summary>
        public virtual ICollection<PagePublishStatusQuery> PagePublishStatusQueries { get; set; } = new List<PagePublishStatusQuery>();

        /// <summary>
        /// <para>
        /// Access rules are used to restrict access to a website resource to users
        /// fulfilling certain criteria such as a specific user area or role. Page
        /// access rules are used to define the rules at a <see cref="Page"/> level, 
        /// however rules are also inherited from the directories the page is parented to.
        /// </para>
        /// <para>
        /// Note that access rules do not apply to users from the Cofoundry Admin user
        /// area. They aren't intended to be used to restrict editor access in the admin UI 
        /// but instead are used to restrict public access to website pages and routes.
        /// </para>
        /// </summary>
        public virtual ICollection<PageAccessRule> AccessRules { get; set; } = new List<PageAccessRule>();

        public int GetId()
        {
            return PageId;
        }
    }
}