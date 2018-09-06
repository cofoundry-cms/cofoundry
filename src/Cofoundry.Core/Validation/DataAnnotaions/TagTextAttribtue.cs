using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// Validates a string field to contain text that you would typically only
    /// want to see in a 'tag' or 'keyword' field. It must only contain only 
    /// letters, numbers whitespace, apostrophes, ampsands and round brackets.
    /// E.g. "Space &amp; Exploration", "Jenny's Music", "Music (Pop)"
    /// </summary>
    public class TagTextAttribtue : RegularExpressionAttribute
    {
        public TagTextAttribtue()
            : base(@"^[&\w\s'()-]+$")
        {
            ErrorMessage = "The {0} field contains invalid characters.";
        }
    }
}
