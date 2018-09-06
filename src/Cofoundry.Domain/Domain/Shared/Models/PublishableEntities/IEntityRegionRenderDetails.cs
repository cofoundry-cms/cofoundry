using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Abstraction of a renderable page region which could either
    /// be a standard page region or a custom entity page region.
    /// </summary>
    /// <typeparam name="TBlockRenderDetails">
    /// Type of block data, which will be specific to the entity being rendered.
    /// </typeparam>
    public interface IEntityRegionRenderDetails<TBlockRenderDetails> where TBlockRenderDetails : IEntityVersionPageBlockRenderDetails
    {
        /// <summary>
        /// Database id of the page template region record.
        /// </summary>
        int PageTemplateRegionId { get; set; }

        /// <summary>
        /// The name identifier for a region. Region names can be any text string 
        /// but will likely be alpha-numerical human readable names like 'Heading', 
        /// 'Main Content'. Region names are unique (non-case sensitive) for the 
        /// template they belong to.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Collection of fully mapped and ordered blocks including display models.
        /// </summary>
        ICollection<TBlockRenderDetails> Blocks { get; set; }
    }
}
