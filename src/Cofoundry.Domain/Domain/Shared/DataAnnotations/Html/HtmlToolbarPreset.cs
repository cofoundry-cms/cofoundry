using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a category of toolbar buttons to add to an 
    /// html editor for the HtmlAttribute property
    /// </summary>
    public enum HtmlToolbarPreset
    {
        /// <summary>
        /// Toolbar items for h1-3
        /// </summary>
        Headings,

        /// <summary>
        /// Toolbar items for bold, italic, underline, links
        /// </summary>
        BasicFormatting,

        /// <summary>
        /// Toolbar items for alignment, blockquote & lists
        /// </summary>
        AdvancedFormatting,

        /// <summary>
        /// Insert pictures, video
        /// </summary>
        Media,

        /// <summary>
        /// Edit html source
        /// </summary>
        Source,

        /// <summary>
        /// Indicates the position to insert a custom toolbar
        /// </summary>
        Custom
    }
}
