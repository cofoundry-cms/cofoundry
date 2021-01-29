using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the UrlSlug and locale of a custom entity which often forms
    /// the identity of the entity and can form part fo the url when used in
    /// custom entity pages. This is a specific action that can
    /// have specific side effects such as breaking page links outside
    /// of the CMS.
    /// </summary>
    public class UpdateCustomEntityUrlCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Database id of the custom entity to update.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }

        /// <summary>
        /// The string identifier slug which can
        /// be used as a lookup identifier or in the routing 
        /// of the custom entity page. Can be forced to be unique
        /// by a setting on the custom entity definition.
        /// </summary>
        [Required]
        [StringLength(200)]
        [Slug]
        public virtual string UrlSlug { get; set; }

        /// <summary>
        /// Optional id of a locale to assign to the custom entity
        /// if used in a localized site.
        /// </summary>
        [PositiveInteger]
        public int? LocaleId { get; set; }
    }
}
