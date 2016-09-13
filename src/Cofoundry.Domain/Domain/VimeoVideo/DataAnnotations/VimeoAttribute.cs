using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to decorate image properties in module data providers to give properties about the image for filtering when browsing.
    /// I.e. you can specify dimensions and tags for filtering the list of images.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VimeoAttribute : Attribute, IMetadataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VimeoAttribute"/> class.
        /// </summary>
        public VimeoAttribute()
            : base()
        {
            
        }

        public void Process(ModelMetadata modelMetaData)
        {
            modelMetaData.TemplateHint = "Vimeo";
        }
    }
}