using Cofoundry.Domain;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Cofoundry.Web.Admin.TagHelpers.Internal
{
    /// <summary>
    /// Adds password policy attributes to a password input.
    /// </summary>
    /// <remarks>
    /// Currently marked as pubternal as it's not clear whether this is useful or 
    /// whether we should be providing tag helpers like this, because the UI layer 
    /// and any associated validation is implementation specific.
    /// </remarks>
    [HtmlTargetElement("input", Attributes = "cf-policy,[type=password]", TagStructure = TagStructure.WithoutEndTag)]
    public class PasswordPolicyTagHelper : TagHelper
    {
        [HtmlAttributeName("cf-policy")]
        public PasswordPolicyDescription Policy { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Policy == null)
            {
                throw new InvalidOperationException(nameof(Policy) + " cannot be null.");
            }

            foreach (var attribute in Policy.Attributes)
            {
                if (!output.Attributes.ContainsName(attribute.Key))
                {
                    output.Attributes.SetAttribute(attribute.Key, attribute.Value);
                }
            }

            if (!output.Attributes.ContainsName("title"))
            {
                output.Attributes.SetAttribute("title", Policy.Description);
            }

            base.Process(context, output);
        }
    }
}
